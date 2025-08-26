using RoutinesGymService.Application.DataTransferObject.SplitDay.UpdateSplitDay;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Application.Interface.Repository;

namespace RoutinesGymService.Application.UseCase
{
    public class SplitDayApplication : ISplitDayApplication
    {
        private readonly ISplitDayRepository _splitDayRepository;

        public SplitDayApplication(ISplitDayRepository splitDayRepository)
        {
            _splitDayRepository = splitDayRepository;
        }

        public async Task<UpdateSplitDayResponse> UpdateSplitDay(UpdateSplitDayRequest updateSplitDayRequest)
        {
            return await _splitDayRepository.UpdateSplitDay(updateSplitDayRequest);
        }
    }
}