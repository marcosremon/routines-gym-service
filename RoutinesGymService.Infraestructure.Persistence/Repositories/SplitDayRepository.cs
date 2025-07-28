using Microsoft.EntityFrameworkCore;
using RoutinesGymService.Application.DataTransferObject.SplitDay.DeleteSplitDay;
using RoutinesGymService.Application.DataTransferObject.SplitDay.UpdateSplitDay;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Infraestructure.Persistence.Context;

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
                                s.DayName == deleteSplitDayRequest.DayName);
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

        public async Task<UpdateSplitDayResponse> UpdateSplitDay(UpdateSplitDayRequest actualizarSplitDayRequest)
        {
            throw new NotImplementedException();
        }
    }
}