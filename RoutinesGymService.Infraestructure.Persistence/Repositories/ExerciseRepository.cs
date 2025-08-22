using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExercise;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExerciseProgress;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.DeleteExercise;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.GetExercisesByDayAndRoutineId;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.UpdateExercise;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetAllUserRoutines;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Application.Mapper;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Infraestructure.Persistence.Context;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly int _expiryMinutes;
        private readonly GenericUtils _genericUtils;
        private readonly string _exercisePrefix;

        public ExerciseRepository(ApplicationDbContext context, GenericUtils genericUtils, IConfiguration configuration, IMemoryCache cache)
        {
            _cache = cache;
            _context = context;
            _genericUtils = genericUtils;
            _exercisePrefix = configuration["CacheSettings:ExercisePrefix"]!;
            _expiryMinutes = int.TryParse(configuration["CacheSettings:CacheExpiryMinutes"], out var m) ? m : 60;
        }

        public async Task<AddExerciseResponse> AddExercise(AddExerciseRequest addExerciseRequest)
        {

            AddExerciseResponse addExerciseResponse = new AddExerciseResponse();
            IDbContextTransaction dbContextTransaction = _context.Database.BeginTransaction();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == addExerciseRequest.UserEmail);
                if (user == null)
                {
                    addExerciseResponse.IsSuccess = false;
                    addExerciseResponse.Message = "User not found.";
                }
                else
                {
                    Routine? routine = await _context.Routines.FirstOrDefaultAsync(r => 
                        r.RoutineId == addExerciseRequest.RoutineId &&
                        r.UserId == user.UserId);
                    if (routine == null)
                    {
                        addExerciseResponse.IsSuccess = false;
                        addExerciseResponse.Message = "Routine not found.";
                    }
                    else
                    {
                        SplitDay? splitDay = await _context.SplitDays.FirstOrDefaultAsync(s =>
                            s.RoutineId == addExerciseRequest.RoutineId &&
                            s.DayName == GenericUtils.ChangeEnumToIntOnDayName(GenericUtils.ChangeStringToEnumOnDayName(addExerciseRequest.DayName)));
                        if (splitDay == null)
                        {
                            addExerciseResponse.IsSuccess = false;
                            addExerciseResponse.Message = "Split day not found.";
                        }
                        else
                        {
                            Exercise? exercise = _context.Exercises.FirstOrDefault(e =>
                                e.RoutineId == routine.RoutineId &&
                                e.SplitDayId == splitDay.SplitDayId &&
                                e.ExerciseName == addExerciseRequest.ExerciseName);
                            if (exercise != null)
                            {
                                addExerciseResponse.IsSuccess = false;
                                addExerciseResponse.Message = "Exercise already exists for this routine and day.";
                            }
                            else
                            {
                                Exercise newExercise = new Exercise
                                {
                                    RoutineId = routine.RoutineId,
                                    SplitDayId = splitDay.SplitDayId,
                                    ExerciseName = addExerciseRequest.ExerciseName
                                };

                                _context.Exercises.Add(newExercise);
                                await _context.SaveChangesAsync();

                                splitDay.Exercises.Add(newExercise);
                                await _context.SaveChangesAsync();

                                await dbContextTransaction.CommitAsync();

                                addExerciseResponse.IsSuccess = true;
                                addExerciseResponse.Message = "Exercise added successfully.";
                            }
                        }
                    }
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

        public async Task<DeleteExerciseResponse> DeleteExercise(DeleteExerciseRequest deleteExerciseRequest)
        {
            DeleteExerciseResponse deleteExerciseResponse = new DeleteExerciseResponse();
            IDbContextTransaction dbContextTransaction = _context.Database.BeginTransaction();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == deleteExerciseRequest.UserEmail);
                if (user == null)
                {
                    deleteExerciseResponse.IsSuccess = false;
                    deleteExerciseResponse.Message = "User not found.";
                }
                else
                {
                    Routine? routine = await _context.Routines.FirstOrDefaultAsync(r => 
                        r.RoutineId == deleteExerciseRequest.RoutineId &&
                        r.UserId == user.UserId);
                    if (routine == null)
                    {
                        deleteExerciseResponse.IsSuccess = false;
                        deleteExerciseResponse.Message = "Routine not found.";
                    }
                    else
                    {
                        SplitDay? splitDay = await _context.SplitDays.FirstOrDefaultAsync(s =>
                            s.RoutineId == deleteExerciseRequest.RoutineId &&
                            s.DayName == GenericUtils.ChangeEnumToIntOnDayName(GenericUtils.ChangeStringToEnumOnDayName(deleteExerciseRequest.DayName!)));
                        if (splitDay == null)
                        {
                            deleteExerciseResponse.IsSuccess = false;
                            deleteExerciseResponse.Message = "Split day not found.";
                        }
                        else
                        {
                            Exercise? exercise = await _context.Exercises.FirstOrDefaultAsync(e =>
                                e.RoutineId == routine.RoutineId &&
                                e.SplitDayId == splitDay.SplitDayId &&
                                e.ExerciseId == deleteExerciseRequest.ExerciseId);
                            if (exercise == null)
                            {
                                deleteExerciseResponse.IsSuccess = false;
                                deleteExerciseResponse.Message = "Exercise not found.";
                            }
                            else
                            {
                                _context.Exercises.Remove(exercise);
                                await _context.SaveChangesAsync();
                            
                                splitDay.Exercises.Remove(exercise);
                                await _context.SaveChangesAsync();

                                List<ExerciseProgress> exerciseProgresses = await _context.ExerciseProgress
                                    .Where(ep => ep.ExerciseId == exercise.ExerciseId)
                                    .ToListAsync(); 
                                _context.ExerciseProgress.RemoveRange(exerciseProgresses);
                                await _context.SaveChangesAsync();

                                _genericUtils.ClearCache(_exercisePrefix);

                                await dbContextTransaction.CommitAsync();

                                deleteExerciseResponse.IsSuccess = true;
                                deleteExerciseResponse.Message = "Exercise deleted successfully.";
                            }
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

        public async Task<GetExercisesByDayAndRoutineIdResponse> GetExercisesByDayAndRoutineId(GetExercisesByDayAndRoutineIdRequest getExercisesByDayNameRequest)
        {
            GetExercisesByDayAndRoutineIdResponse getExercisesByDayAndRoutineIdResponse = new GetExercisesByDayAndRoutineIdResponse();
            try
            {
                string cacheKey = $"{_exercisePrefix}GetExercisesByDayAndRoutineId{getExercisesByDayNameRequest.RoutineId}_{getExercisesByDayNameRequest.DayName}";

                GetExercisesByDayAndRoutineIdResponse? cacheExercise = _cache.Get<GetExercisesByDayAndRoutineIdResponse>(cacheKey);

                if (cacheExercise != null)
                {
                    getExercisesByDayAndRoutineIdResponse.Exercises = cacheExercise.Exercises;
                    getExercisesByDayAndRoutineIdResponse.PastProgress = cacheExercise.PastProgress;
                    getExercisesByDayAndRoutineIdResponse.IsSuccess = cacheExercise.IsSuccess;
                    getExercisesByDayAndRoutineIdResponse.Message = cacheExercise.Message;
                }
                else
                {
                    Routine? routine = await _context.Routines.FirstOrDefaultAsync(r => r.RoutineId == getExercisesByDayNameRequest.RoutineId);
                    if (routine == null)
                    {
                        getExercisesByDayAndRoutineIdResponse.IsSuccess = false;
                        getExercisesByDayAndRoutineIdResponse.Message = "Routine not found.";
                    }
                    else
                    {
                        SplitDay? splitDay = await _context.SplitDays.FirstOrDefaultAsync(s =>
                            s.RoutineId == getExercisesByDayNameRequest.RoutineId &&
                            s.DayName == GenericUtils.ChangeEnumToIntOnDayName(GenericUtils.ChangeStringToEnumOnDayName(getExercisesByDayNameRequest.DayName!)));
                        if (splitDay == null)
                        {
                            getExercisesByDayAndRoutineIdResponse.IsSuccess = false;
                            getExercisesByDayAndRoutineIdResponse.Message = "Split day not found.";
                        }
                        else
                        {
                            List<Exercise> exercises = await _context.Exercises
                                .Where(e =>
                                    e.RoutineId == routine.RoutineId &&
                                    e.SplitDayId == splitDay.SplitDayId)
                                .ToListAsync();
                            if (exercises.Count == 0)
                            {
                                getExercisesByDayAndRoutineIdResponse.IsSuccess = false;
                                getExercisesByDayAndRoutineIdResponse.Message = "No exercises found for this routine and day.";
                            }
                            else
                            {
                                Dictionary<long, List<string>> pastProgressDict = new Dictionary<long, List<string>>();
                                foreach (Exercise exercise in exercises)
                                {
                                    List<ExerciseProgress> last3Progress = await _context.ExerciseProgress
                                       .Where(p => p.ExerciseId == exercise.ExerciseId &&
                                            p.RoutineId == splitDay.RoutineId &&
                                            p.DayName == splitDay.DayName.ToString())
                                       .OrderByDescending(p => p.PerformedAt)
                                       .Take(3)
                                       .ToListAsync();

                                    List<string> pastProgress = last3Progress
                                        .Select(p => $"{p.Sets}x{p.Reps}@{p.Weight}kg")
                                        .ToList();

                                    pastProgressDict[exercise.ExerciseId] = pastProgress;
                                }

                                getExercisesByDayAndRoutineIdResponse.Exercises = exercises.Select(e => ExerciseMapper.ExerciseToDto(e)).ToList();
                                getExercisesByDayAndRoutineIdResponse.PastProgress = pastProgressDict;
                                getExercisesByDayAndRoutineIdResponse.IsSuccess = true;
                                getExercisesByDayAndRoutineIdResponse.Message = "Exercises retrieved successfully.";

                                _cache.Set(cacheKey, getExercisesByDayAndRoutineIdResponse, TimeSpan.FromMinutes(_expiryMinutes));
                            }
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

        public async Task<UpdateExerciseResponse> UpdateExercise(UpdateExerciseRequest updateExerciseRequest)
        {
            UpdateExerciseResponse updateExerciseResponse = new UpdateExerciseResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == updateExerciseRequest.UserId);
                if (user == null)
                {
                    updateExerciseResponse.IsSuccess = false;
                    updateExerciseResponse.Message = "User not found";
                }
                else
                {
                    Routine? routine = await _context.Routines.FirstOrDefaultAsync(r => 
                        r.RoutineId == updateExerciseRequest.RoutineId &&
                        r.UserId == user.UserId);
                    if (routine == null)
                    {
                        updateExerciseResponse.IsSuccess = false;
                        updateExerciseResponse.Message = "Routine not found.";
                    }
                    else
                    {
                        SplitDay? splitDay = await _context.SplitDays.FirstOrDefaultAsync(s =>
                            s.RoutineId == updateExerciseRequest.RoutineId &&
                            s.DayName == GenericUtils.ChangeEnumToIntOnDayName(updateExerciseRequest.DayName));
                        if (splitDay == null)
                        {
                            updateExerciseResponse.IsSuccess = false;
                            updateExerciseResponse.Message = "Split day not found.";
                        }
                        else
                        {
                            Exercise? exercise = await _context.Exercises.FirstOrDefaultAsync(e =>
                                e.RoutineId == routine.RoutineId &&
                                e.SplitDayId == splitDay.SplitDayId &&
                                e.ExerciseName == updateExerciseRequest.ExerciseName);
                            if (exercise == null)
                            {
                                updateExerciseResponse.IsSuccess = false;
                                updateExerciseResponse.Message = "Exercise not found.";
                            }
                            else
                            {
                                // to_do

                                // Update the exercise properties
                                //exercise.ExerciseName = updateExerciseRequest.ExerciseName ?? exercise.ExerciseName;
                                //exercise.Sets = updateExerciseRequest.Sets ?? exercise.Sets;
                                //exercise.Reps = updateExerciseRequest.Reps ?? exercise.Reps;
                                //exercise.Weight = updateExerciseRequest.Weight ?? exercise.Weight;
                                //_context.Exercises.Update(exercise);
                                //await _context.SaveChangesAsync();
                                //updateExerciseResponse.IsSuccess = true;
                                //updateExerciseResponse.UserDTO = UserMapper.UserToDto(user);
                                //updateExerciseResponse.Message = "Exercise updated successfully.";


                                // añadir el remove cache
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                updateExerciseResponse.IsSuccess = false;
                updateExerciseResponse.Message = $"unexpected error on ExerciseRepository -> UpdateExercise: {ex.Message}";
            }
            
            return updateExerciseResponse;
        }

        public async Task<AddExerciseAddExerciseProgressResponse> AddExerciseProgress(AddExerciseAddExerciseProgressRequest addExerciseAddExerciseProgressRequest)
        {
            AddExerciseAddExerciseProgressResponse addExerciseAddExerciseProgressResponse = new AddExerciseAddExerciseProgressResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == addExerciseAddExerciseProgressRequest.UserEmail);
                if (user == null)
                {
                    addExerciseAddExerciseProgressResponse.IsSuccess = false;
                    addExerciseAddExerciseProgressResponse.Message = "User not found.";
                }
                else
                {
                    Routine? routine = await _context.Routines.FirstOrDefaultAsync(r =>
                        r.RoutineId == addExerciseAddExerciseProgressRequest.RoutineId &&
                        r.UserId == user.UserId);
                    if (routine == null)
                    {
                        addExerciseAddExerciseProgressResponse.IsSuccess = false;
                        addExerciseAddExerciseProgressResponse.Message = "Routine not found.";
                    }
                    else
                    {
                        SplitDay? splitDay = await _context.SplitDays.FirstOrDefaultAsync(s =>
                            s.RoutineId == addExerciseAddExerciseProgressRequest.RoutineId &&
                            s.DayName == GenericUtils.ChangeEnumToIntOnDayName(GenericUtils.ChangeStringToEnumOnDayName(addExerciseAddExerciseProgressRequest.DayName!)));
                        if (splitDay == null)
                        {
                            addExerciseAddExerciseProgressResponse.IsSuccess = false;
                            addExerciseAddExerciseProgressResponse.Message = "Split day not found.";
                        }
                        else
                        {
                            // to_do
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
    }
}