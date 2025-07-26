using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Application.DataTransferObject.SplitDay.GetAllUserSplits
{
    public class GetAllUserSplitsResponse : BaseResponse
    {
        public List<SplitDayDTO> SplitDays { get; set; } = new List<SplitDayDTO>();
    }
}