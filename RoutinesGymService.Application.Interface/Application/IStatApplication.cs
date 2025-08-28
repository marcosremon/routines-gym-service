using RoutinesGymService.Application.DataTransferObject.Interchange.Stat.GetDailyStepsInfo;
using RoutinesGymService.Application.DataTransferObject.Interchange.Stat.GetStats;
using RoutinesGymService.Transversal.JsonInterchange.Stat.SaveDailySteps;

namespace RoutinesGymService.Application.Interface.Application
{
    public interface IStatApplication
    {
        Task<GetDailyStepsInfoResponse> GetDailyStepsInfo(GetDailyStepsInfoRequest getDailyStepsInfoRequest);
        Task<GetStatsResponse> GetStats(GetStatRequest getStatRequest);
        Task<SaveDailyStepsResponse> SaveDailySteps(SaveDailyStepsRequest saveDailyStepsRequest);
    }
}