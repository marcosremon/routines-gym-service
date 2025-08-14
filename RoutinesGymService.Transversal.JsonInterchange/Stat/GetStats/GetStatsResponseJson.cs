using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Transversal.JsonInterchange.Stat.GetStats
{
    public class GetStatsResponseJson : BaseResponseJson
    {
        public List<RoutinesGymApp.Domain.Entities.Stat>? Stats { get; set; }
    }
}