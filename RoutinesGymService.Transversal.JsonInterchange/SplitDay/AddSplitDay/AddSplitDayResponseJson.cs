using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Transversal.JsonInterchange.SplitDay.AddSplitDay
{
    public class AddSplitDayResponseJson : BaseResponseJson
    {
        public UserDTO? UserDTO { get; set; }
    }
}