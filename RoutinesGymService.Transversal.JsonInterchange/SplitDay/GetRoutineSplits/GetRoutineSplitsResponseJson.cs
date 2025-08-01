using RoutinesGymService.Application.DataTransferObject.Interchange.SplitDay.GetRoutineSplits;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Transversal.JsonInterchange.SplitDay.GetRoutineSplits
{
    public class GetRoutineSplitsResponseJson : BaseResponseJson
    {
        public List<DayInfoDTO> DayInfo { get; set; } = new List<DayInfoDTO>();
    }
}