using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;
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
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class RoutineRepository : IRoutineRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly int _expiryMinutes;
        private readonly GenericUtils _genericUtils;
        private readonly string _routinePrefix;

        public RoutineRepository(ApplicationDbContext context, GenericUtils genericUtils, IConfiguration configuration, IMemoryCache cache)
        {
            _cache = cache;
            _context = context;
            _genericUtils = genericUtils;
            _routinePrefix = configuration["CacheSettings:RoutinePrefix"]!;
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
                                    splitDay.Exercises.Add(new Exercise
                                    {
                                        ExerciseName = exerciseRequest.ExerciseName,
                                        RoutineId = routine.RoutineId,
                                    });
                                }

                                routine.SplitDays.Add(splitDay);
                            }

                            await _context.SaveChangesAsync();

                            foreach (SplitDay splitDay in routine.SplitDays)
                            {
                                foreach (Exercise exercise in splitDay.Exercises)
                                {
                                    ExerciseProgress progress = new ExerciseProgress
                                    {
                                        ExerciseId = exercise.ExerciseId,
                                        RoutineId = routine.RoutineId,
                                        DayName = splitDay.DayNameString,
                                        PerformedAt = DateTime.UtcNow
                                    };

                                    _context.ExerciseProgress.Add(progress);
                                }
                            }

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
                }
                else
                {
                    Routine? routine = await _context.Routines.FirstOrDefaultAsync(r => r.RoutineId == deleteRoutineRequest.RoutineId && r.UserId == user.UserId);
                    if (routine == null)
                    {
                        deleteRoutineResponse.IsSuccess = false;
                        deleteRoutineResponse.Message = "Routine not found for the user";
                    }
                    else
                    {
                        if (!user.Routines.Contains(routine))
                        {
                            deleteRoutineResponse.IsSuccess = false;
                            deleteRoutineResponse.Message = "This routine does not belong to the user";
                        }
                        else
                        {
                            _context.Routines.Remove(routine);
                            _genericUtils.ClearCache(_routinePrefix);

                            await _context.SaveChangesAsync();

                            deleteRoutineResponse.IsSuccess = true;
                            deleteRoutineResponse.Message = "Routine deleted successfully";
                        }
                    }
                }
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
                string cacheKey = $"{_routinePrefix}_GetAllUserRoutines_{getAllUserRoutinesRequest.UserEmail}";

                List<Routine>? cacheRoutines = _cache.Get<List<Routine>>(cacheKey);
                if (cacheRoutines != null)
                {
                    getAllUserRoutinesResponse.IsSuccess = true;
                    getAllUserRoutinesResponse.Message = "Routines retrieved successfully";
                    getAllUserRoutinesResponse.Routines = cacheRoutines.Select(r => RoutineMapper.RoutineToDto(r)).ToList();
                }
                else
                {
                    User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == getAllUserRoutinesRequest.UserEmail);
                    if (user == null)
                    {
                        getAllUserRoutinesResponse.IsSuccess = false;
                        getAllUserRoutinesResponse.Message = "User not found";
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

                            _cache.Set(cacheKey, routines, TimeSpan.FromMinutes(_expiryMinutes));
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

        public async Task<GetRoutineByIdResponse> GetRoutineById(GetRoutineByIdRequest getRoutineByIdRequest)
        {
            GetRoutineByIdResponse getRoutineByIdResponse = new GetRoutineByIdResponse();
            try
            {
                string cacheKey = $"{_routinePrefix}_GetRoutineById_{getRoutineByIdRequest.RoutineId}";

                Routine? cachedRoutine = _cache.Get<Routine>(cacheKey);
                if (cachedRoutine != null)
                {
                    getRoutineByIdResponse.IsSuccess = true;
                    getRoutineByIdResponse.Message = "Routine retrieved successfully from cache";
                    getRoutineByIdResponse.RoutineDTO = RoutineMapper.RoutineToDto(cachedRoutine);
                }
                else
                {
                    Routine? routine = await _context.Routines.FirstOrDefaultAsync(r => r.RoutineId == getRoutineByIdRequest.RoutineId);
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

                        _cache.Set(cacheKey, routine, TimeSpan.FromMinutes(_expiryMinutes));
                    }
                }
            }
            catch (Exception ex)
            {
                getRoutineByIdResponse.Message = $"unexpected error on RoutineRepository -> GetRoutineById {ex.Message}";
                getRoutineByIdResponse.IsSuccess = false;
            }

            return getRoutineByIdResponse;
        }

        public async Task<GetRoutineStatsResponse> GetRoutineStats(GetRoutineStatsRequest getRoutineStatsRequest)
        {
            GetRoutineStatsResponse getRoutineStatsResponse = new GetRoutineStatsResponse();

            try
            {
                string cacheKey = $"{_routinePrefix}_GetRoutineStats_{getRoutineStatsRequest.UserEmail}";

                GetRoutineStatsResponse? cachedStats = _cache.Get<GetRoutineStatsResponse>(cacheKey);
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
                    User? user = await _context.Users
                        .Include(u => u.Routines)
                            .ThenInclude(r => r.SplitDays)
                                .ThenInclude(sd => sd.Exercises)
                        .FirstOrDefaultAsync(u => u.Email == getRoutineStatsRequest.UserEmail);

                    if (user == null)
                    {
                        getRoutineStatsResponse.IsSuccess = false;
                        getRoutineStatsResponse.Message = "User not found";
                    }
                    else
                    {
                        List<Routine> routines = user.Routines.ToList();

                        if (!routines.Any())
                        {
                            getRoutineStatsResponse.RoutinesCount = 0;
                            getRoutineStatsResponse.SplitsCount = 0;
                            getRoutineStatsResponse.ExercisesCount = 0;
                            getRoutineStatsResponse.IsSuccess = true;
                            getRoutineStatsResponse.Message = "No routines found for the user";
                        }
                        else
                        {
                            List<SplitDay> splitDays = routines.SelectMany(r => r.SplitDays).ToList();

                            if (!splitDays.Any())
                            {
                                getRoutineStatsResponse.RoutinesCount = routines.Count;
                                getRoutineStatsResponse.SplitsCount = 0;
                                getRoutineStatsResponse.ExercisesCount = 0;
                                getRoutineStatsResponse.IsSuccess = true;
                                getRoutineStatsResponse.Message = "No split days found for the user's routines";
                            }
                            else
                            {
                                List<Exercise> exercises = splitDays.SelectMany(sd => sd.Exercises).ToList();

                                getRoutineStatsResponse.RoutinesCount = routines.Count;
                                getRoutineStatsResponse.SplitsCount = splitDays.Count;
                                getRoutineStatsResponse.ExercisesCount = exercises.Count;
                                getRoutineStatsResponse.IsSuccess = true;
                                getRoutineStatsResponse.Message = exercises.Any()
                                    ? "Routine stats retrieved successfully"
                                    : "No exercises found for the user's split days";
                            }
                        }

                        _cache.Set(cacheKey, getRoutineStatsResponse, TimeSpan.FromMinutes(_expiryMinutes));
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