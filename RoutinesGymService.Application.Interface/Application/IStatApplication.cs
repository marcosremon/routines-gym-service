using RoutinesGymService.Application.DataTransferObject.Interchange.Stat.GetStats;

namespace RoutinesGymService.Application.Interface.Application
{
    public interface IStatApplication
    {
        Task<GetStatsResponse> GetStats();
    }
}