using RoutinesGymService.Application.DataTransferObject.Interchange.Stat.GetStats;

namespace RoutinesGymService.Application.Interface.Repository
{
    public interface IStatRepository
    {
        Task<GetStatsResponse> GetStats();
    }
}