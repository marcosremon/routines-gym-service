using RoutinesGymService.Application.DataTransferObject.Interchange.SplitDay.GetRoutineSplits;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.SplitDay.GetRoutineSplits
{
    public class GetRoutineSplitsResponse : BaseResponse
    {
        public List<DayInfoDTO> DayInfo { get; set; } = new List<DayInfoDTO>();
    }
}