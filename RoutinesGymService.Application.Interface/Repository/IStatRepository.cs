using RoutinesGymService.Application.DataTransferObject.Interchange.Stat.GetStats;
using RoutinesGymService.Transversal.JsonInterchange.Stat.SaveDailySteps;

namespace RoutinesGymService.Application.Interface.Repository
{
    public interface IStatRepository
    {
        Task<GetStatsResponse> GetStats(GetStatRequest getStatRequest);
        Task<SaveDailyStepsResponse> SaveDailySteps(SaveDailyStepsRequest saveDailyStepsRequest);
    }
}