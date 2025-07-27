using RoutinesGymService.Application.DataTransferObject.SplitDay.DeleteSplitDay;
using RoutinesGymService.Application.DataTransferObject.SplitDay.UpdateSplitDay;
using RoutinesGymService.Application.Interface.Repository;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class SplitDayRepository : ISplitDayRepository
    {
        public Task<DeleteSplitDayResponse> DeleteSplitDay(DeleteSplitDayRequest deleteSplitDayRequest)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateSplitDayResponse> UpdateSplitDay(UpdateSplitDayRequest actualizarSplitDayRequest)
        {
            throw new NotImplementedException();
        }
    }
}