using RoutinesGymService.Application.DataTransferObject.Interchange.Stat.GetStats;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Application.Interface.Repository;

namespace RoutinesGymService.Application.UseCase
{
    public class StatApplication : IStatApplication
    {
        private readonly IStatRepository _statRepository;

        public StatApplication(IStatRepository statRepository)
        {
            _statRepository = statRepository;
        }

        public async Task<GetStatsResponse> GetStats()
        {
            return await _statRepository.GetStats();
        }
    }
}