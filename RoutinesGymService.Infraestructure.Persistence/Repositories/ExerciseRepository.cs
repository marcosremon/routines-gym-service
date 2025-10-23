using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExercise;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExerciseProgress;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.DeleteExercise;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.ExerciseExistInThisWeekday;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.GetAllExerciseProgress;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.GetExercisesByDayAndRoutineId;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Application.Mapper;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Infraestructure.Persistence.Context;
using RoutinesGymService.Transversal.Common.Utils;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly GenericUtils _genericUtils;
        private readonly CacheUtils _cacheUtils;
        private readonly int _expiryMinutes;
        private readonly string _exercisePrefix;

        public ExerciseRepository(ApplicationDbContext context, GenericUtils genericUtils, CacheUtils cacheUtils, IConfiguration configuration)
        {
            _cacheUtils = cacheUtils;
            _context = context;
            _genericUtils = genericUtils;
            _exercisePrefix = configuration["CacheSettings:ExercisePrefix"]!;
            _expiryMinutes = int.TryParse(configuration["CacheSettings:CacheExpiryMinutes"], out var m) ? m : 60;
        }

        #region Add exercise
        public async Task<AddExerciseResponse> AddExercise(AddExerciseRequest addExerciseRequest)
        {
            AddExerciseResponse addExerciseResponse = new AddExerciseResponse();
            await using IDbContextTransaction dbContextTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                ExerciseExistInThisWeekdayRequest exerciseExistInThisWeekdayRequest = new ExerciseExistInThisWeekdayRequest
                {
                    RoutineName = addExerciseRequest.RoutineName,
                    DayName = addExerciseRequest.DayName,
                    ExerciseName = addExerciseRequest.ExerciseName,
                    UserEmail = addExerciseRequest.UserEmail
                };

                ExerciseExistInThisWeekdayResponse exerciseExistInThisWeekdayResponse = await ExerciseExistInThisWeekday(exerciseExistInThisWeekdayRequest);

                if ((exerciseExistInThisWeekdayResponse.RoutineId == -1 || 
                     exerciseExistInThisWeekdayResponse.SplitDayId == -1) &&
                    !exerciseExistInThisWeekdayResponse.IsSuccess)
                {
                    addExerciseResponse.IsSuccess = false;
                    addExerciseResponse.Message = exerciseExistInThisWeekdayResponse.Message;
                }
                else if (exerciseExistInThisWeekdayResponse.IsSuccess)
                {
                    addExerciseResponse.IsSuccess = false;
                    addExerciseResponse.Message = "Exercise already exists for this routine and day";
                }
                else
                {
                    Exercise newExercise = new Exercise
                    {
                        RoutineId = exerciseExistInThisWeekdayResponse.RoutineId,
                        SplitDayId = exerciseExistInThisWeekdayResponse.SplitDayId,
                        ExerciseName = addExerciseRequest.ExerciseName
                    };

                    _genericUtils.ClearCache(_exercisePrefix);

                    _context.Exercises.Add(newExercise);
                    await _context.SaveChangesAsync();

                    await dbContextTransaction.CommitAsync();

                    addExerciseResponse.IsSuccess = true;
                    addExerciseResponse.Message = "Exercise added successfully";
                }
            }
            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync();
                addExerciseResponse.IsSuccess = false;
                addExerciseResponse.Message = $"unexpected error on ExerciseRepository -> AddExercise: {ex.Message}";
            }

            return addExerciseResponse;
        }
        #endregion

        #region Delete exercise
        public async Task<DeleteExerciseResponse> DeleteExercise(DeleteExerciseRequest deleteExerciseRequest)
        {
            DeleteExerciseResponse deleteExerciseResponse = new DeleteExerciseResponse();
            await using IDbContextTransaction dbContextTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Routine routine = await _context.Routines.FirstAsync(r => r.RoutineId == deleteExerciseRequest.RoutineId);
                if (routine == null)
                {
                    deleteExerciseResponse.IsSuccess = false;
                    deleteExerciseResponse.Message = "Routine not found";
                }
                else
                {
                    ExerciseExistInThisWeekdayRequest exerciseExistInThisWeekdayRequest = new ExerciseExistInThisWeekdayRequest
                    {
                        UserEmail = deleteExerciseRequest.UserEmail,
                        RoutineName = routine.RoutineName,
                        DayName = deleteExerciseRequest.DayName,
                        ExerciseName = deleteExerciseRequest.ExerciseName
                    };

                    ExerciseExistInThisWeekdayResponse checkResponse = await ExerciseExistInThisWeekday(exerciseExistInThisWeekdayRequest);

                    if (!checkResponse.IsSuccess)
                    {
                        deleteExerciseResponse.IsSuccess = false;
                        deleteExerciseResponse.Message = checkResponse.Message;
                    }
                    else
                    {
                        Exercise? exercise = await _context.Exercises.FirstOrDefaultAsync(e =>
                            e.RoutineId == checkResponse.RoutineId &&
                            e.SplitDayId == checkResponse.SplitDayId &&
                            e.ExerciseName == deleteExerciseRequest.ExerciseName);
                        if (exercise != null)
                        {
                            List<ExerciseProgress> exerciseProgresses = await _context.ExerciseProgress
                                .Where(ep => ep.ExerciseId == exercise.ExerciseId)
                                .ToListAsync();

                            _genericUtils.ClearCache(_exercisePrefix);

                            _context.ExerciseProgress.RemoveRange(exerciseProgresses);
                            await _context.SaveChangesAsync();

                            _context.Exercises.Remove(exercise);
                            await _context.SaveChangesAsync();

                            await dbContextTransaction.CommitAsync();

                            deleteExerciseResponse.IsSuccess = true;
                            deleteExerciseResponse.Message = "Exercise deleted successfully.";
                        }
                        else
                        {
                            deleteExerciseResponse.IsSuccess = false;
                            deleteExerciseResponse.Message = "Exercise not found during deletion";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync();
                deleteExerciseResponse.IsSuccess = false;
                deleteExerciseResponse.Message = $"unexpected error on ExerciseRepository -> DeleteExercise: {ex.Message}";
            }
            return deleteExerciseResponse;
        }
        #endregion

        #region Get exercises by day and routine name
        public async Task<GetExercisesByDayAndRoutineNameResponse> GetExercisesByDayAndRoutineName(GetExercisesByDayAndRoutineNameRequest getExercisesByDayAndRoutineNameRequest)
        {
            GetExercisesByDayAndRoutineNameResponse getExercisesByDayAndRoutineIdResponse = new GetExercisesByDayAndRoutineNameResponse();
            try
            {
                string cacheKey = $"{_exercisePrefix}GetExercisesByDayAndRoutineName_{getExercisesByDayAndRoutineNameRequest.RoutineName}_{getExercisesByDayAndRoutineNameRequest.DayName}";

                GetExercisesByDayAndRoutineNameResponse? cacheExercise = _cacheUtils.Get<GetExercisesByDayAndRoutineNameResponse>(cacheKey);
                if (cacheExercise != null)
                {
                    getExercisesByDayAndRoutineIdResponse.Exercises = cacheExercise.Exercises;
                    getExercisesByDayAndRoutineIdResponse.PastProgress = cacheExercise.PastProgress;
                    getExercisesByDayAndRoutineIdResponse.IsSuccess = cacheExercise.IsSuccess;
                    getExercisesByDayAndRoutineIdResponse.Message = cacheExercise.Message;
                }
                else
                {
                    ExerciseExistInThisWeekdayRequest validationRequest = new ExerciseExistInThisWeekdayRequest
                    {
                        UserEmail = getExercisesByDayAndRoutineNameRequest.UserEmail,
                        RoutineName = getExercisesByDayAndRoutineNameRequest.RoutineName,
                        DayName = getExercisesByDayAndRoutineNameRequest.DayName,
                        ExerciseName = ""
                    };

                    ExerciseExistInThisWeekdayResponse validationResponse = await ExerciseExistInThisWeekday(validationRequest);
                    if (!validationResponse.IsSuccess &&
                         validationResponse.RoutineId == -1)
                    {
                        getExercisesByDayAndRoutineIdResponse.IsSuccess = false;
                        getExercisesByDayAndRoutineIdResponse.Message = validationResponse.Message;
                    }
                    else
                    {
                        List<Exercise> exercises = await _context.Exercises
                           .Where(e => e.RoutineId == validationResponse.RoutineId &&
                                       e.SplitDayId == validationResponse.SplitDayId)
                           .ToListAsync();
                        if (!exercises.Any())
                        {
                            getExercisesByDayAndRoutineIdResponse.IsSuccess = false;
                            getExercisesByDayAndRoutineIdResponse.Message = "No exercises found for this routine and day.";
                        }
                        else
                        {
                            int dayNameToInt = GenericUtils.ChangeEnumToIntOnDayName(GenericUtils.ChangeStringToEnumOnDayName(getExercisesByDayAndRoutineNameRequest.DayName));
                            Dictionary<string, List<string>> pastProgressDict = new Dictionary<string, List<string>>();
                            foreach (Exercise exercise in exercises)
                            {
                                List<ExerciseProgress> last3Progress = await _context.ExerciseProgress
                                    .Where(p => p.ExerciseId == exercise.ExerciseId &&
                                        p.RoutineId == validationResponse.RoutineId &&
                                        p.DayName == GenericUtils.ChangeIntToEnumOnDayName(dayNameToInt).ToString())
                                    .OrderByDescending(p => p.PerformedAt)
                                    .Take(3)
                                    .Reverse()
                                    .ToListAsync();

                                List<string> pastProgress = last3Progress
                                    .Select(p => $"{p.Sets}x{p.Reps}@{p.Weight}kg")
                                    .ToList();

                                pastProgressDict[exercise.ExerciseName] = pastProgress;
                            }

                            getExercisesByDayAndRoutineIdResponse.Exercises = exercises.Select(e => ExerciseMapper.ExerciseToDto(e)).ToList();
                            getExercisesByDayAndRoutineIdResponse.PastProgress = pastProgressDict;
                            getExercisesByDayAndRoutineIdResponse.IsSuccess = true;
                            getExercisesByDayAndRoutineIdResponse.Message = "Exercises retrieved successfully.";

                            _cacheUtils.Set(cacheKey, getExercisesByDayAndRoutineIdResponse, TimeSpan.FromMinutes(_expiryMinutes));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                getExercisesByDayAndRoutineIdResponse.IsSuccess = false;
                getExercisesByDayAndRoutineIdResponse.Message = $"unexpected error on ExerciseRepository -> GetExercisesByDayAndRoutineId: {ex.Message}";
            }

            return getExercisesByDayAndRoutineIdResponse;
        }
        #endregion

        #region Add exercise progress
        public async Task<AddExerciseAddExerciseProgressResponse> AddExerciseProgress(AddExerciseAddExerciseProgressRequest addExerciseAddExerciseProgressRequest)
        {
            AddExerciseAddExerciseProgressResponse addExerciseAddExerciseProgressResponse = new AddExerciseAddExerciseProgressResponse();
            try
            {
                Routine? routine = await _context.Routines.FirstOrDefaultAsync(r => r.RoutineId == addExerciseAddExerciseProgressRequest.RoutineId);
                SplitDay? splitDay = await _context.SplitDays.FirstOrDefaultAsync(s => s.SplitDayId == addExerciseAddExerciseProgressRequest.splitDayId);

                if (routine == null)
                {
                    addExerciseAddExerciseProgressResponse.IsSuccess = false;
                    addExerciseAddExerciseProgressResponse.Message = "Routine not found.";
                }
                else if (splitDay == null)
                {
                    addExerciseAddExerciseProgressResponse.IsSuccess = false;
                    addExerciseAddExerciseProgressResponse.Message = "Split day not found.";
                }
                else
                {
                    ExerciseExistInThisWeekdayRequest validationRequest = new ExerciseExistInThisWeekdayRequest
                    {
                        UserEmail = addExerciseAddExerciseProgressRequest.UserEmail,
                        RoutineName = routine.RoutineName,
                        DayName = GenericUtils.ChangeIntToEnumOnDayName(splitDay.DayName).ToString(),
                        ExerciseName = addExerciseAddExerciseProgressRequest.ExerciseName
                    };

                    ExerciseExistInThisWeekdayResponse validationResponse = await ExerciseExistInThisWeekday(validationRequest);
                    if (!validationResponse.IsSuccess && validationResponse.RoutineId == -1)
                    {
                        addExerciseAddExerciseProgressResponse.IsSuccess = false;
                        addExerciseAddExerciseProgressResponse.Message = validationResponse.Message;
                    }
                    else if (!validationResponse.IsSuccess)
                    {
                        addExerciseAddExerciseProgressResponse.IsSuccess = false;
                        addExerciseAddExerciseProgressResponse.Message = "Exercise not found.";
                    }
                    else
                    {
                        Exercise? exercise = await _context.Exercises.FirstOrDefaultAsync(e =>
                            e.ExerciseName == addExerciseAddExerciseProgressRequest.ExerciseName &&
                            e.RoutineId == validationResponse.RoutineId &&
                            e.SplitDayId == validationResponse.SplitDayId);

                        if (exercise == null)
                        {
                            addExerciseAddExerciseProgressResponse.IsSuccess = false;
                            addExerciseAddExerciseProgressResponse.Message = "Exercise not found.";
                        }
                        else
                        {
                            string weightRaw = addExerciseAddExerciseProgressRequest.ProgressList[2];
                            string weight = weightRaw.Any(c => c == 46) // en ASCII el '.' es el numero 46
                                ? weightRaw.Replace(".", ",")
                                : weightRaw;

                            ExerciseProgress exerciseProgress = new ExerciseProgress
                            {
                                ExerciseId = exercise.ExerciseId,
                                RoutineId = validationResponse.RoutineId,
                                DayName = GenericUtils.ChangeIntToEnumOnDayName(splitDay.DayName).ToString(),
                                Sets = int.Parse(addExerciseAddExerciseProgressRequest.ProgressList[0]),
                                Reps = int.Parse(addExerciseAddExerciseProgressRequest.ProgressList[1]),
                                Weight = float.Parse(weight),
                                PerformedAt = DateTime.UtcNow,
                            };

                            _genericUtils.ClearCache(_exercisePrefix);

                            await _context.ExerciseProgress.AddAsync(exerciseProgress);
                            await _context.SaveChangesAsync();

                            addExerciseAddExerciseProgressResponse.IsSuccess = true;
                            addExerciseAddExerciseProgressResponse.Message = "exercise progress added successfully";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                addExerciseAddExerciseProgressResponse.IsSuccess = false;
                addExerciseAddExerciseProgressResponse.Message = $"unexpected error on ExerciseRepository -> AddExerciseProgress: {ex.Message}";
            }

            return addExerciseAddExerciseProgressResponse;
        }
        #endregion

        #region Get all exercise progress
        public async Task<GetAllExerciseProgressResponse> GetAllExerciseProgress(GetAllExerciseProgressRequest getAllExerciseProgressRequest)
        {
            GetAllExerciseProgressResponse getAllExerciseProgressResponse = new GetAllExerciseProgressResponse();
            try
            {
                string cacheKey = $"{_exercisePrefix}GetAllExerciseProgress{getAllExerciseProgressRequest.UserEmail}_{getAllExerciseProgressRequest.ExerciseName}_{getAllExerciseProgressRequest.RoutineName}_{getAllExerciseProgressRequest.DayName}";

                GetAllExerciseProgressResponse? cacheExercise = _cacheUtils.Get<GetAllExerciseProgressResponse>(cacheKey);
                if (cacheExercise != null)
                {
                    getAllExerciseProgressResponse.ExerciseProgressList = cacheExercise.ExerciseProgressList;
                    getAllExerciseProgressResponse.IsSuccess = cacheExercise.IsSuccess;
                    getAllExerciseProgressResponse.Message = cacheExercise.Message;
                }
                else
                {
                    ExerciseExistInThisWeekdayRequest validationRequest = new ExerciseExistInThisWeekdayRequest
                    {
                        UserEmail = getAllExerciseProgressRequest.UserEmail,
                        RoutineName = getAllExerciseProgressRequest.RoutineName,
                        DayName = getAllExerciseProgressRequest.DayName,
                        ExerciseName = getAllExerciseProgressRequest.ExerciseName
                    };

                    ExerciseExistInThisWeekdayResponse validationResponse = await ExerciseExistInThisWeekday(validationRequest);
                    if (!validationResponse.IsSuccess && validationResponse.RoutineId == -1)
                    {
                        getAllExerciseProgressResponse.IsSuccess = false;
                        getAllExerciseProgressResponse.Message = validationResponse.Message;
                    }
                    else if (!validationResponse.IsSuccess)
                    {
                        getAllExerciseProgressResponse.IsSuccess = false;
                        getAllExerciseProgressResponse.Message = "Exercise not found";
                    }
                    else
                    {
                        Exercise? exercise = await _context.Exercises.FirstOrDefaultAsync(e =>
                            e.ExerciseName == getAllExerciseProgressRequest.ExerciseName &&
                            e.RoutineId == validationResponse.RoutineId &&
                            e.SplitDayId == validationResponse.SplitDayId);
                        if (exercise == null)
                        {
                            getAllExerciseProgressResponse.IsSuccess = false;
                            getAllExerciseProgressResponse.Message = "Exercise not found";
                        }
                        else
                        {
                            List<ExerciseProgress> exerciseProgressList = await _context.ExerciseProgress
                                .Where(ep => ep.ExerciseId == exercise.ExerciseId &&
                                             ep.RoutineId == validationResponse.RoutineId)
                                .OrderByDescending(ep => ep.PerformedAt)
                                .ToListAsync();

                            getAllExerciseProgressResponse.IsSuccess = true;
                            getAllExerciseProgressResponse.Message = "Exercise progress retrieved successfully.";
                            getAllExerciseProgressResponse.ExerciseProgressList = exerciseProgressList
                                .Select(ep => ExerciseMapper.ExerciseProgressToDto(ep))
                                .ToList();

                            _cacheUtils.Set(cacheKey, getAllExerciseProgressResponse, TimeSpan.FromMinutes(_expiryMinutes));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                getAllExerciseProgressResponse.IsSuccess = false;
                getAllExerciseProgressResponse.Message = $"unexpected error on ExerciseRepository -> GetAllExerciseProgress: {ex.Message}";
            }

            return getAllExerciseProgressResponse;
        }
        #endregion

        #region Auxiliary methods

        #region ExerciseExistInThisWeekday
        public async Task<ExerciseExistInThisWeekdayResponse> ExerciseExistInThisWeekday(ExerciseExistInThisWeekdayRequest exerciseExistInThisWeekdayRequest)
        {
            ExerciseExistInThisWeekdayResponse exerciseExistInThisWeekdayResponse = new ExerciseExistInThisWeekdayResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == exerciseExistInThisWeekdayRequest.UserEmail);
                if (user == null)
                {
                    exerciseExistInThisWeekdayResponse.IsSuccess = false;
                    exerciseExistInThisWeekdayResponse.Message = "User not found";
                }
                else
                {
                    Routine? routine = await _context.Routines.FirstOrDefaultAsync(r => 
                        r.RoutineName == exerciseExistInThisWeekdayRequest.RoutineName &&
                        r.UserId == user.UserId);
                    if (routine == null)
                    {
                        exerciseExistInThisWeekdayResponse.IsSuccess = false;
                        exerciseExistInThisWeekdayResponse.Message = "Routine not found";
                    }
                    else
                    {
                        string day = exerciseExistInThisWeekdayRequest.DayName.Contains(".")
                            ? exerciseExistInThisWeekdayRequest.DayName.Split(".")[1]
                            : exerciseExistInThisWeekdayRequest.DayName;

                        int dayToInt = GenericUtils.ChangeEnumToIntOnDayName(GenericUtils.ChangeStringToEnumOnDayName(day));

                        SplitDay? splitDay = await _context.SplitDays.FirstOrDefaultAsync(s => 
                            s.RoutineId == routine.RoutineId &&
                            s.DayName == dayToInt);
                        if (splitDay == null)
                        {
                            exerciseExistInThisWeekdayResponse.IsSuccess = false;
                            exerciseExistInThisWeekdayResponse.Message = "Split day not found";
                        }
                        else
                        {
                            Exercise? exercise = _context.Exercises.FirstOrDefault(e => 
                                e.RoutineId == routine.RoutineId &&
                                e.SplitDayId == splitDay.SplitDayId &&
                                e.ExerciseName == exerciseExistInThisWeekdayRequest.ExerciseName);
                            if (exercise != null)
                            {
                                exerciseExistInThisWeekdayResponse.IsSuccess = true;
                                exerciseExistInThisWeekdayResponse.Message = "Exercise already exists for this routine and day";
                            }
                            else
                            {
                                exerciseExistInThisWeekdayResponse.IsSuccess = false;
                                exerciseExistInThisWeekdayResponse.Message = "The exercise does not exist in that routine for that day of the week.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exerciseExistInThisWeekdayResponse.IsSuccess = false;
                exerciseExistInThisWeekdayResponse.Message = $"unexpected error on ExerciseRepository -> ExerciseExistInThisWeekday: {ex.Message}";
            }

            return exerciseExistInThisWeekdayResponse;
        }
        #endregion

        #endregion
    }
}