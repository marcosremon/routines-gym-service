using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Stat.GetStats
{
    public class GetStatsResponse : BaseResponse
    {
        public List<RoutinesGymApp.Domain.Entities.Stat>? Stats { get; set; }
    }
}
