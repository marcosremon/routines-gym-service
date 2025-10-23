using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RoutinesGymService.Application.DataTransferObject.SplitDay.UpdateSplitDay;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Application.Mapper;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Domain.Model.Enums;
using RoutinesGymService.Infraestructure.Persistence.Context;
using RoutinesGymService.Transversal.Common.Utils;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class SplitDayRepository : ISplitDayRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly GenericUtils _genericUtils;
        private readonly string _userPrefix;
        private readonly string _routinePrefix;

        public SplitDayRepository(ApplicationDbContext context, GenericUtils genericUtils, IConfiguration configuration)
        {
            _context = context;
            _genericUtils = genericUtils;
            _userPrefix = configuration["CacheSettings:UserPrefix"]!;
            _routinePrefix = configuration["CacheSettings:RoutinePrefix"]!;
        }

        #region Update split day
        public async Task<UpdateSplitDayResponse> UpdateSplitDay(UpdateSplitDayRequest updateSplitDayRequest)
        {
            UpdateSplitDayResponse updateSplitDayResponse = new UpdateSplitDayResponse();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == updateSplitDayRequest.UserEmail);
                if (user == null)
                {
                    updateSplitDayResponse.IsSuccess = false;
                    updateSplitDayResponse.Message = "User not found";
                }
                else
                {
                    Routine? routine = await _context.Routines
                        .FirstOrDefaultAsync(r =>
                            r.RoutineName == updateSplitDayRequest.RoutineName &&
                            r.UserId == user.UserId);

                    if (routine == null)
                    {
                        updateSplitDayResponse.IsSuccess = false;
                        updateSplitDayResponse.Message = "Routine not found or does not belong to this user";
                    }
                    else if (!updateSplitDayRequest.DeleteDays.Any() && !updateSplitDayRequest.AddDays.Any())
                    {
                        updateSplitDayResponse.IsSuccess = false;
                        updateSplitDayResponse.Message = "No days to delete or add";
                    }
                    else
                    {
                        bool hasChanges = false;

                        if (updateSplitDayRequest.DeleteDays.Count > 0)
                        {
                            foreach (string dayName in updateSplitDayRequest.DeleteDays)
                            {
                                string normalizedDayName = GenericUtils.ChangeDayLanguage_sp_to_eng(dayName).ToLower();

                                SplitDay? splitDayToDelete = await _context.SplitDays
                                    .FirstOrDefaultAsync(sd =>
                                        sd.RoutineId == routine.RoutineId &&
                                        sd.DayNameString.ToLower() == normalizedDayName);

                                if (splitDayToDelete != null)
                                {
                                    List<long> exerciseIds = await _context.Exercises
                                        .Join(_context.SplitDays,
                                            e => e.SplitDayId,
                                            sd => sd.SplitDayId,
                                            (e, sd) => new { Exercise = e, SplitDay = sd })
                                        .Where(x => x.SplitDay.SplitDayId == splitDayToDelete.SplitDayId)
                                        .Select(x => x.Exercise.ExerciseId)
                                        .ToListAsync();

                                    if (exerciseIds.Any())
                                    {
                                        List<ExerciseProgress> exerciseProgresses = await _context.ExerciseProgress
                                            .Where(ep => exerciseIds.Contains(ep.ExerciseId))
                                            .ToListAsync();

                                        _context.ExerciseProgress.RemoveRange(exerciseProgresses);
                                    }

                                    List<Exercise> exercises = await _context.Exercises
                                        .Where(e => e.SplitDayId == splitDayToDelete.SplitDayId)
                                        .ToListAsync();

                                    _context.Exercises.RemoveRange(exercises);

                                    _context.SplitDays.Remove(splitDayToDelete);
                                    hasChanges = true;
                                }
                            }
                        }

                        if (updateSplitDayRequest.AddDays.Count > 0)
                        {
                            List<string> existingSplitDays = await _context.SplitDays
                                .Join(_context.Routines,
                                    sd => sd.RoutineId,
                                    r => r.RoutineId,
                                    (sd, r) => new { SplitDay = sd, Routine = r })
                                .Where(x => x.Routine.RoutineId == routine.RoutineId)
                                .Select(x => x.SplitDay.DayNameString.ToLower())
                                .ToListAsync();

                            foreach (string dayName in updateSplitDayRequest.AddDays)
                            {
                                string normalizedDayName = GenericUtils.ChangeDayLanguage_sp_to_eng(dayName);

                                if (!existingSplitDays.Contains(normalizedDayName.ToLower()))
                                {
                                    WeekDay weekDay = Enum.Parse<WeekDay>(normalizedDayName, true);
                                    SplitDay newSplitDay = new SplitDay
                                    {
                                        DayName = GenericUtils.ChangeEnumToIntOnDayName(weekDay),
                                        DayNameString = normalizedDayName,
                                        RoutineId = routine.RoutineId,
                                        Exercises = new List<Exercise>()
                                    };
                                    _context.SplitDays.Add(newSplitDay);
                                    hasChanges = true;
                                }
                            }
                        }

                        if (hasChanges)
                        {
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();

                            _genericUtils.ClearCache($"{_userPrefix}:{user.UserId}");
                            _genericUtils.ClearCache($"{_routinePrefix}:{routine.RoutineId}");

                            User? updatedUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);
                            if (updatedUser != null)
                            {
                                List<Routine> userRoutines = await _context.Routines
                                    .Join(_context.Users,
                                        r => r.UserId,
                                        u => u.UserId,
                                        (r, u) => new { Routine = r, User = u })
                                    .Where(x => x.User.UserId == updatedUser.UserId)
                                    .Select(x => x.Routine)
                                    .ToListAsync();

                                updatedUser.Routines = userRoutines;

                                foreach (Routine userRoutine in updatedUser.Routines)
                                {
                                    List<SplitDay> routineSplitDays = await _context.SplitDays
                                        .Join(_context.Routines,
                                            sd => sd.RoutineId,
                                            r => r.RoutineId,
                                            (sd, r) => new { SplitDay = sd, Routine = r })
                                        .Where(x => x.Routine.RoutineId == userRoutine.RoutineId)
                                        .Select(x => x.SplitDay)
                                        .ToListAsync();

                                    userRoutine.SplitDays = routineSplitDays;
                                }

                                updateSplitDayResponse.UserDto = UserMapper.UserToDto(updatedUser);
                            }

                            updateSplitDayResponse.IsSuccess = true;
                            updateSplitDayResponse.Message = "Split day updated successfully";
                        }
                        else
                        {
                            await transaction.RollbackAsync();
                            updateSplitDayResponse.IsSuccess = false;
                            updateSplitDayResponse.Message = "No changes were made";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                updateSplitDayResponse.IsSuccess = false;
                updateSplitDayResponse.Message = $"Unexpected error on SplitDayRepository -> UpdateSplitDay: {ex.Message}";
            }

            return updateSplitDayResponse;
        }
        #endregion
    }
}