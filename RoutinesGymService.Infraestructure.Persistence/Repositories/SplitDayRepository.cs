using RoutinesGymService.Application.DataTransferObject.SplitDay.DeleteSplitDay;
using RoutinesGymService.Application.DataTransferObject.SplitDay.UpdateSplitDay;
using RoutinesGymService.Application.Interface.Repository;
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
            throw new NotImplementedException();
        }

        public async Task<UpdateSplitDayResponse> UpdateSplitDay(UpdateSplitDayRequest actualizarSplitDayRequest)
        {
            throw new NotImplementedException();
        }
    }
}