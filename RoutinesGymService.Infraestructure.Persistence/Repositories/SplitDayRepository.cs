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

        public async Task<UpdateSplitDayResponse> UpdateSplitDay(UpdateSplitDayRequest updateSplitDayRequest)
        {
            UpdateSplitDayResponse updateSplitDayResponse = new UpdateSplitDayResponse();

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
                    Routine? routine = await _context.Routines.FirstOrDefaultAsync(r =>
                        r.RoutineName == updateSplitDayRequest.RoutineName &&
                        r.UserId == user.UserId);
                    if (routine == null)
                    {
                        updateSplitDayResponse.IsSuccess = false;
                        updateSplitDayResponse.Message = "Routine not found";
                    }
                    else
                    {
                        if (!user.Routines.Any(r => r.RoutineId == routine.RoutineId))
                        {
                            updateSplitDayResponse.IsSuccess = false;
                            updateSplitDayResponse.Message = "User does not have this routine";
                        }
                        else
                        {
                            if (updateSplitDayRequest.DeleteDays.Count == 0 && updateSplitDayRequest.AddDays.Count == 0)
                            {
                                updateSplitDayResponse.IsSuccess = false;
                                updateSplitDayResponse.Message = "No days to delete or add";
                            }
                            else
                            {
                                if (updateSplitDayRequest.DeleteDays.Count > 0)
                                {
                                    updateSplitDayRequest.DeleteDays.ForEach(dayName =>
                                    {
                                        dayName = GenericUtils.ChangeDayLanguage_sp_to_eng(dayName).ToLower();

                                        var splitDayToDelete = _context.SplitDays
                                            .Where(sd => sd.RoutineId == routine.RoutineId && sd.DayNameString.ToLower() == dayName)
                                            .Join(_context.Exercises, sd => sd.SplitDayId, e => e.SplitDayId, (sd, e) => new { SplitDay = sd, Exercise = e })
                                            .GroupBy(x => x.SplitDay)
                                            .Select(g => new
                                            {
                                                SplitDay = g.Key,
                                                Exercises = g.Select(x => x.Exercise).ToList()
                                            })
                                            .FirstOrDefault();

                                        if (splitDayToDelete != null)
                                        {
                                            List<long> exerciseIds = splitDayToDelete.Exercises.Select(e => e.ExerciseId).ToList();

                                            IQueryable<ExerciseProgress> progressToDelete = _context.ExerciseProgress
                                                .Where(ep => exerciseIds.Contains(ep.ExerciseId));

                                            _context.ExerciseProgress.RemoveRange(progressToDelete);
                                            _context.Exercises.RemoveRange(splitDayToDelete.Exercises);
                                            _context.SplitDays.Remove(splitDayToDelete.SplitDay);
                                        }
                                    });
                                }

                                if (updateSplitDayRequest.AddDays.Count > 0)
                                {
                                    updateSplitDayRequest.AddDays.ForEach(dayName =>
                                    {
                                        dayName = GenericUtils.ChangeDayLanguage_sp_to_eng(dayName);

                                        WeekDay weekDay = Enum.Parse<WeekDay>(dayName, true);
                                        SplitDay newSplitDay = new SplitDay
                                        {
                                            DayName = GenericUtils.ChangeEnumToIntOnDayName(weekDay),
                                            DayNameString = dayName,
                                            RoutineId = routine.RoutineId,
                                            Exercises = new List<Exercise>()
                                        };
                                        routine.SplitDays.Add(newSplitDay);
                                    });
                                }

                                _genericUtils.ClearCache(_userPrefix);
                                _genericUtils.ClearCache(_routinePrefix);

                                await _context.SaveChangesAsync();

                                updateSplitDayResponse.IsSuccess = true;
                                updateSplitDayResponse.Message = "Split day updated successfully.";
                                updateSplitDayResponse.UserDTO = UserMapper.UserToDto(user);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                updateSplitDayResponse.IsSuccess = false;
                updateSplitDayResponse.Message = $"unexpected error on SplitDayRepository -> DeleteSplitDay: {ex.Message}";
            }
            return updateSplitDayResponse;
        }
    }
}