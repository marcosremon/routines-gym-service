using RoutinesGymService.Application.DataTransferObject.SplitDay.UpdateSplitDay;

namespace RoutinesGymService.Application.Interface.Repository
{
    public interface ISplitDayRepository
    {
        Task<UpdateSplitDayResponse> UpdateSplitDay(UpdateSplitDayRequest updateSplitDayRequest);
    }
}