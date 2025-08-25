using RoutinesGymService.Application.DataTransferObject.Interchange.Stat.GetStats;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Transversal.JsonInterchange.Stat.SaveDailySteps;

namespace RoutinesGymService.Application.UseCase
{
    public class StatApplication : IStatApplication
    {
        private readonly IStatRepository _statRepository;

        public StatApplication(IStatRepository statRepository)
        {
            _statRepository = statRepository;
        }

        public async Task<GetStatsResponse> GetStats(GetStatRequest getStatRequest)
        {
            return await _statRepository.GetStats(getStatRequest);
        }

        public async Task<SaveDailyStepsResponse> SaveDailySteps(SaveDailyStepsRequest saveDailyStepsRequest)
        {
            return await _statRepository.SaveDailySteps(saveDailyStepsRequest);
        }
    }
}