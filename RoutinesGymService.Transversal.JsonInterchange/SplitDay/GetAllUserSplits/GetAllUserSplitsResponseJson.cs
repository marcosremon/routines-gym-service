using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Transversal.JsonInterchange.SplitDay.GetAllUserSplits
{
    public class GetAllUserSplitsResponseJson : BaseResponseJson
    {
        public List<SplitDayDTO> SplitDays { get; set; } = new List<SplitDayDTO>();
    }
}