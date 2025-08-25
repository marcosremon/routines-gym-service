using Microsoft.EntityFrameworkCore;
using RoutinesGymService.Application.DataTransferObject.SplitDay.DeleteSplitDay;
using RoutinesGymService.Application.DataTransferObject.SplitDay.UpdateSplitDay;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Application.Mapper;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Domain.Model.Enums;
using RoutinesGymService.Infraestructure.Persistence.Context;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class SplitDayRepository : ISplitDayRepository
    {
        private readonly ApplicationDbContext _context;

        public SplitDayRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DeleteSplitDayResponse> DeleteSplitDay(DeleteSplitDayRequest deleteSplitDayRequest)
        {
            DeleteSplitDayResponse deleteSplitDayResponse = new DeleteSplitDayResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == deleteSplitDayRequest.UserId);
                if (user == null)
                {
                    deleteSplitDayResponse.IsSuccess = false;
                    deleteSplitDayResponse.Message = "user not found";
                }
                else
                {
                    Routine? routine = await _context.Routines.FirstOrDefaultAsync(r => r.RoutineId == deleteSplitDayRequest.RoutineId);
                    if (routine == null)
                    {
                        deleteSplitDayResponse.IsSuccess = false;
                        deleteSplitDayResponse.Message = "Routine not found";
                    }
                    else
                    {
                        if (!user.Routines.Any(r => r.UserId == user.UserId))
                        {
                            deleteSplitDayResponse.IsSuccess = false;
                            deleteSplitDayResponse.Message = "User does not have this routine";
                        }
                        else
                        {
                            SplitDay? splitDay = await _context.SplitDays.FirstOrDefaultAsync(s =>
                                s.RoutineId == deleteSplitDayRequest.RoutineId &&
                                s.DayName == GenericUtils.ChangeEnumToIntOnDayName(deleteSplitDayRequest.DayName));
                            if (splitDay == null)
                            {
                                deleteSplitDayResponse.IsSuccess = false;
                                deleteSplitDayResponse.Message = "Split day not found";
                            }
                            else
                            {
                                if (!routine.SplitDays.Any(r => r.SplitDayId == splitDay.SplitDayId))
                                {
                                    deleteSplitDayResponse.IsSuccess = false;
                                    deleteSplitDayResponse.Message = "Routine does not have this split day";
                                }
                                else
                                {
                                    _context.SplitDays.Remove(splitDay);
                                    await _context.SaveChangesAsync();

                                    deleteSplitDayResponse.IsSuccess = true;
                                    deleteSplitDayResponse.Message = "Split day deleted successfully";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                deleteSplitDayResponse.IsSuccess = false;
                deleteSplitDayResponse.Message = $"unexpected error on SplitDayRepository -> DeleteSplitDay: {ex.Message}";
            }
            
            return deleteSplitDayResponse;
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
                                        dayName = GenericUtils.ChangeDayLanguage(dayName).ToLower();

                                        SplitDay? splitDayToDelete = _context.SplitDays
                                            .Include(sd => sd.Exercises)
                                            .FirstOrDefault(sd =>
                                                sd.RoutineId == routine.RoutineId &&
                                                sd.DayNameString.ToLower() == dayName);
                                        if (splitDayToDelete != null)
                                        {
                                            List<long> exerciseIds = splitDayToDelete.Exercises.Select(e => e.ExerciseId).ToList();

                                            IQueryable<ExerciseProgress> progressToDelete = _context.ExerciseProgress.Where(ep => exerciseIds.Contains(ep.ExerciseId));
                                            _context.ExerciseProgress.RemoveRange(progressToDelete);
                                            _context.Exercises.RemoveRange(splitDayToDelete.Exercises);
                                            _context.SplitDays.Remove(splitDayToDelete);
                                        }
                                    });
                                }

                                if (updateSplitDayRequest.AddDays.Count > 0)
                                {
                                    updateSplitDayRequest.AddDays.ForEach(dayName =>
                                    {
                                        dayName = GenericUtils.ChangeDayLanguage(dayName);

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