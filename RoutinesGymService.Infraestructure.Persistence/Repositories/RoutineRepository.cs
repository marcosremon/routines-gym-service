using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.CreateRoutine;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.DeleteRoutine;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetAllUserRoutines;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineById;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineStats;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.UpdateRoutine;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Application.Mapper;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Infraestructure.Persistence.Context;
using RoutinesGymService.Transversal.Common.Utils;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class RoutineRepository : IRoutineRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly CacheUtils _cacheUtils;
        private readonly GenericUtils _genericUtils;
        private readonly int _expiryMinutes;
        private readonly string _routinePrefix;
        private readonly string _exercisePrefix;

        public RoutineRepository(ApplicationDbContext context, GenericUtils genericUtils, CacheUtils cacheUtils, IConfiguration configuration)
        {
            _cacheUtils = cacheUtils;
            _context = context;
            _genericUtils = genericUtils;
            _routinePrefix = configuration["CacheSettings:RoutinePrefix"]!;
            _exercisePrefix = configuration["CacheSettings:ExercisePrefix"]!;
            _expiryMinutes = int.TryParse(configuration["CacheSettings:CacheExpiryMinutes"], out var m) ? m : 60;
        }

        public async Task<CreateRoutineResponse> CreateRoutine(CreateRoutineRequest createRoutineRequest)
        {
            CreateRoutineResponse createRoutineResponse = new CreateRoutineResponse();
            IDbContextTransaction dbContextTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == createRoutineRequest.UserEmail);
                if (user == null)
                {
                    createRoutineResponse.Message = "User not found";
                    createRoutineResponse.IsSuccess = false;
                }
                else
                {
                    if (user.Routines.Any(r => r.RoutineName == createRoutineRequest.RoutineName))
                    {
                        createRoutineResponse.Message = "Routine with this name already exists for the user";
                        createRoutineResponse.IsSuccess = false;
                    }
                    else
                    {
                        bool hasInvalidExercise = false;
                        foreach (SplitDayDTO splitDay in createRoutineRequest.SplitDays)
                        {
                            foreach (ExerciseDTO exercise in splitDay.Exercises)
                            {
                                if (exercise == null || string.IsNullOrWhiteSpace(exercise.ExerciseName))
                                {
                                    hasInvalidExercise = true;
                                    break;
                                }
                            }
                            if (hasInvalidExercise) 
                                break;
                        }

                        if (hasInvalidExercise)
                        {
                            createRoutineResponse.Message = "Exercise name cannot be null or empty";
                            createRoutineResponse.IsSuccess = false;
                        }
                        else
                        {
                            Routine routine = new Routine
                            {
                                RoutineName = createRoutineRequest.RoutineName!,
                                RoutineDescription = string.IsNullOrEmpty(createRoutineRequest.RoutineDescription)
                                    ? "sin descripcion"
                                    : createRoutineRequest.RoutineDescription,
                                UserId = user.UserId,
                                SplitDays = new List<SplitDay>()
                            };

                            _context.Routines.Add(routine);
                            await _context.SaveChangesAsync();

                            foreach (SplitDayDTO requestSplitDay in createRoutineRequest.SplitDays)
                            {
                                SplitDay splitDay = new SplitDay
                                {
                                    DayName = GenericUtils.ChangeEnumToIntOnDayName(requestSplitDay.DayName),
                                    DayNameString = requestSplitDay.DayName.ToString(),
                                    DayExercisesDescription = "Default description",
                                    RoutineId = routine.RoutineId,
                                    Exercises = new List<Exercise>()
                                };

                                foreach (ExerciseDTO exerciseRequest in requestSplitDay.Exercises)
                                {
                                    Exercise exercise = new Exercise
                                    {
                                        ExerciseName = exerciseRequest.ExerciseName,
                                        RoutineId = routine.RoutineId,
                                    };

                                    splitDay.Exercises.Add(exercise);
                                }

                                routine.SplitDays.Add(splitDay);
                            }

                            await _context.SaveChangesAsync();

                            _genericUtils.ClearCache(_routinePrefix);

                            await _context.SaveChangesAsync();
                            await dbContextTransaction.CommitAsync();

                            createRoutineResponse.IsSuccess = true;
                            createRoutineResponse.Message = "Routine created successfully";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync();
                createRoutineResponse.Message = $"unexpected error on RoutineRepository -> CreateRoutine {ex.Message}";
                createRoutineResponse.IsSuccess = false;
            }

            return createRoutineResponse;
        }

        public async Task<DeleteRoutineResponse> DeleteRoutine(DeleteRoutineRequest deleteRoutineRequest)
        {
            DeleteRoutineResponse deleteRoutineResponse = new DeleteRoutineResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == deleteRoutineRequest.UserEmail);
                if (user == null)
                {
                    deleteRoutineResponse.IsSuccess = false;
                    deleteRoutineResponse.Message = "User not found";
                    return deleteRoutineResponse;
                }

                Routine? routine = await _context.Routines.FirstOrDefaultAsync(r =>
                    r.RoutineName == deleteRoutineRequest.RoutineName &&
                    r.UserId == user.UserId);
                if (routine == null)
                {
                    deleteRoutineResponse.IsSuccess = false;
                    deleteRoutineResponse.Message = "Routine not found for the user";
                    return deleteRoutineResponse;
                }

                if (!user.Routines.Contains(routine))
                {
                    deleteRoutineResponse.IsSuccess = false;
                    deleteRoutineResponse.Message = "This routine does not belong to the user";
                    return deleteRoutineResponse;
                }

                List<SplitDay> splitDays = await _context.SplitDays
                    .Where(sd => sd.RoutineId == routine.RoutineId)
                    .ToListAsync();
                foreach (SplitDay splitDay in splitDays)
                {
                    List<Exercise> exercises = await _context.Exercises
                        .Where(e => e.SplitDayId == splitDay.SplitDayId)
                        .ToListAsync();
                    foreach (Exercise exercise in exercises)
                    {
                        List<ExerciseProgress> progresses = await _context.ExerciseProgress
                            .Where(ep => ep.ExerciseId == exercise.ExerciseId)
                            .ToListAsync();
                        _context.ExerciseProgress.RemoveRange(progresses);
                    }
                    _context.Exercises.RemoveRange(exercises);
                }

                _genericUtils.ClearCache(_routinePrefix);
                _genericUtils.ClearCache(_exercisePrefix);

                _context.SplitDays.RemoveRange(splitDays);
                _context.Routines.Remove(routine);
                await _context.SaveChangesAsync();

                deleteRoutineResponse.IsSuccess = true;
                deleteRoutineResponse.Message = "Routine deleted successfully";
            }
            catch (Exception ex)
            {
                deleteRoutineResponse.Message = $"unexpected error on RoutineRepository -> DeleteRoutine {ex.Message}";
                deleteRoutineResponse.IsSuccess = false;
            }

            return deleteRoutineResponse;
        }

        public async Task<GetAllUserRoutinesResponse> GetAllUserRoutines(GetAllUserRoutinesRequest getAllUserRoutinesRequest)
        {
            GetAllUserRoutinesResponse getAllUserRoutinesResponse = new GetAllUserRoutinesResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == getAllUserRoutinesRequest.UserEmail);
                if (user == null)
                {
                    getAllUserRoutinesResponse.IsSuccess = false;
                    getAllUserRoutinesResponse.Message = "User not found";
                }
                else
                {
                    string cacheKey = $"{_routinePrefix}GetAllUserRoutines_{getAllUserRoutinesRequest.UserEmail}";

                    List<Routine>? cacheRoutines = _cacheUtils.Get<List<Routine>>(cacheKey);
                    if (cacheRoutines != null)
                    {
                        getAllUserRoutinesResponse.IsSuccess = true;
                        getAllUserRoutinesResponse.Message = "Routines retrieved successfully";
                        getAllUserRoutinesResponse.Routines = cacheRoutines.Select(r => RoutineMapper.RoutineToDto(r)).ToList();
                    }
                    else
                    {
                        List<Routine> routines = await _context.Routines.Where(r => r.UserId == user.UserId).ToListAsync();
                        if (routines.Count == 0)
                        {
                            getAllUserRoutinesResponse.IsSuccess = false;
                            getAllUserRoutinesResponse.Message = "No routines found for the user";
                        }
                        else
                        {
                            getAllUserRoutinesResponse.IsSuccess = true;
                            getAllUserRoutinesResponse.Message = "Routines retrieved successfully";
                            getAllUserRoutinesResponse.Routines = routines.Select(r => RoutineMapper.RoutineToDto(r)).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                getAllUserRoutinesResponse.Message = $"unexpected error on RoutineRepository -> GetAllUserRoutines {ex.Message}";
                getAllUserRoutinesResponse.IsSuccess = false;
            }

            return getAllUserRoutinesResponse;
        }

        public async Task<GetRoutineByRoutineNameResponse> GetRoutineByRoutineName(GetRoutineByRoutineNameRequest getRoutineByRoutineNameRequest)
        {
            GetRoutineByRoutineNameResponse getRoutineByIdResponse = new GetRoutineByRoutineNameResponse();

            try
            {
                string cacheKey = $"{_routinePrefix}GetRoutineByRoutineName_{getRoutineByRoutineNameRequest.RoutineName}";

                Routine? cachedRoutine = _cacheUtils.Get<Routine>(cacheKey);
                if (cachedRoutine != null)
                {
                    getRoutineByIdResponse.IsSuccess = true;
                    getRoutineByIdResponse.Message = "Routine retrieved successfully from cache";
                    getRoutineByIdResponse.RoutineDTO = RoutineMapper.RoutineToDto(cachedRoutine);
                }
                else
                {
                    User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == getRoutineByRoutineNameRequest.UserEmail);
                    if (user == null)
                    {
                        getRoutineByIdResponse.IsSuccess = false;
                        getRoutineByIdResponse.Message = "User not found";
                    }
                    else
                    {
                        Routine? routine = await _context.Routines
                            .Where(r => r.RoutineName == getRoutineByRoutineNameRequest.RoutineName && r.UserId == user.UserId)
                            .GroupJoin(
                                _context.SplitDays,
                                r => r.RoutineId,
                                sd => sd.RoutineId,
                                (routine, splitDaysGroup) => new
                                {
                                    Routine = routine,
                                    SplitDaysGroup = splitDaysGroup
                                }
                            )
                            .SelectMany(
                                x => x.SplitDaysGroup.DefaultIfEmpty(), 
                                (parent, child) => new { parent.Routine, SplitDay = child } 
                            )
                            .GroupBy(x => x.Routine)
                            .Select(g => new Routine 
                            {
                                RoutineId = g.Key.RoutineId,
                                RoutineName = g.Key.RoutineName,
                                RoutineDescription = g.Key.RoutineDescription,
                                UserId = g.Key.UserId,
                                SplitDays = g.Select(x => x.SplitDay).Where(sd => sd != null).ToList()
                            })
                            .FirstOrDefaultAsync();

                        if (routine == null)
                        {
                            getRoutineByIdResponse.IsSuccess = false;
                            getRoutineByIdResponse.Message = "Routine not found";
                        }
                        else
                        {
                            getRoutineByIdResponse.RoutineDTO = RoutineMapper.RoutineToDto(routine);
                            getRoutineByIdResponse.IsSuccess = true;
                            getRoutineByIdResponse.Message = "Routine retrieved successfully";

                            _cacheUtils.Set(cacheKey, routine, TimeSpan.FromMinutes(_expiryMinutes));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                getRoutineByIdResponse.Message = $"unexpected error on RoutineRepository -> GetRoutineByRoutineName {ex.Message}";
                getRoutineByIdResponse.IsSuccess = false;
            }

            return getRoutineByIdResponse;
        }

        public async Task<GetRoutineStatsResponse> GetRoutineStats(GetRoutineStatsRequest getRoutineStatsRequest)
        {
            GetRoutineStatsResponse getRoutineStatsResponse = new GetRoutineStatsResponse();

            try
            {
                string cacheKey = $"{_routinePrefix}GetRoutineStats_{getRoutineStatsRequest.UserEmail}";

                GetRoutineStatsResponse? cachedStats = _cacheUtils.Get<GetRoutineStatsResponse>(cacheKey);
                if (cachedStats != null)
                {
                    getRoutineStatsResponse.IsSuccess = cachedStats.IsSuccess;
                    getRoutineStatsResponse.Message = cachedStats.Message;
                    getRoutineStatsResponse.RoutinesCount = cachedStats.RoutinesCount;
                    getRoutineStatsResponse.SplitsCount = cachedStats.SplitsCount;
                    getRoutineStatsResponse.ExercisesCount = cachedStats.ExercisesCount;
                }
                else
                {
                    var userData = await _context.Users
                        .Where(u => u.Email == getRoutineStatsRequest.UserEmail)
                        .Select(u => new
                        {
                            User = u,

                            Routines = u.Routines.ToList(), 

                            SplitDays = u.Routines
                                .SelectMany(r => r.SplitDays) 
                                .Distinct()
                                .ToList(),

                            Exercises = u.Routines
                                .SelectMany(r => r.SplitDays)
                                .SelectMany(sd => sd.Exercises) 
                                .Distinct()
                                .ToList()
                        })
                        .FirstOrDefaultAsync(); 

                    if (userData == null)
                    {
                        getRoutineStatsResponse.IsSuccess = false;
                        getRoutineStatsResponse.Message = "User not found";
                    }
                    else
                    {
                        List<Routine> routines = userData.Routines;
                        List<SplitDay> splitDays = userData.SplitDays;
                        List<Exercise> exercises = userData.Exercises;

                        if (!routines.Any() && !splitDays.Any() && !exercises.Any())
                        {
                            getRoutineStatsResponse.RoutinesCount = 0;
                            getRoutineStatsResponse.SplitsCount = 0;
                            getRoutineStatsResponse.ExercisesCount = 0;
                            getRoutineStatsResponse.IsSuccess = true;
                            getRoutineStatsResponse.Message = "No found for the user";
                        }
                        else
                        {
                            getRoutineStatsResponse.IsSuccess = true;
                            getRoutineStatsResponse.RoutinesCount = routines.Count;
                            getRoutineStatsResponse.SplitsCount = splitDays.Count;
                            getRoutineStatsResponse.ExercisesCount = exercises.Count;
                            getRoutineStatsResponse.Message = "Routine stats retrieved successfully";
                        }

                        _cacheUtils.Set(cacheKey, getRoutineStatsResponse, TimeSpan.FromMinutes(_expiryMinutes));
                    }
                }
            }
            catch (Exception ex)
            {
                getRoutineStatsResponse.IsSuccess = false;
                getRoutineStatsResponse.Message = $"unexpected error on RoutineRepository -> GetRoutineStats: {ex.Message}";
            }

            return getRoutineStatsResponse;
        }

        public async Task<UpdateRoutineResponse> UpdateRoutine(UpdateRoutineRequest updateRoutineRequest)
        {
            UpdateRoutineResponse updateRoutineResponse = new UpdateRoutineResponse();
            try
            {
                Routine? routine = await _context.Routines.FirstOrDefaultAsync(r => r.RoutineId == updateRoutineRequest.RoutineId);
                if (routine == null)
                {
                    updateRoutineResponse.IsSuccess = false;
                    updateRoutineResponse.Message = "Routine not found";
                }
                else
                {
                    routine.RoutineName = updateRoutineRequest.RoutineName ?? routine.RoutineName;
                    routine.RoutineDescription = updateRoutineRequest.RoutineDescription ?? routine.RoutineDescription;
                    routine.SplitDays = updateRoutineRequest.SplitDays.Select(sd => new SplitDay
                    {
                        DayName = GenericUtils.ChangeEnumToIntOnDayName(sd.DayName),
                        Exercises = sd.Exercises.Select(e => new Exercise
                        {
                            ExerciseName = e.ExerciseName
                        }).ToList()
                    }).ToList() ?? routine.SplitDays;

                    _genericUtils.ClearCache(_routinePrefix);
                    await _context.SaveChangesAsync();

                    updateRoutineResponse.IsSuccess = true;
                    updateRoutineResponse.RoutineDTO = RoutineMapper.RoutineToDto(routine);
                    updateRoutineResponse.Message = "Routine updated successfully";
                }
            }
            catch (Exception ex)
            {
                updateRoutineResponse.IsSuccess = false;
                updateRoutineResponse.Message = ex.Message;
            }

            return updateRoutineResponse;
        }
    }
}