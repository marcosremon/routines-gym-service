using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Transversal.JsonInterchange.SplitDay.UpdateSplitDay
{
    public class UpdateSplitDayResponseJson : BaseResponseJson
    {
        public UserDTO? UserDTO { get; set; }
    }
}