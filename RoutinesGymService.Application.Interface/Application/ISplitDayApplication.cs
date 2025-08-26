using RoutinesGymService.Application.DataTransferObject.SplitDay.UpdateSplitDay;

namespace RoutinesGymService.Application.Interface.Application
{
    public interface ISplitDayApplication
    {
        Task<UpdateSplitDayResponse> UpdateSplitDay(UpdateSplitDayRequest updateSplitDayRequest);
    }
}