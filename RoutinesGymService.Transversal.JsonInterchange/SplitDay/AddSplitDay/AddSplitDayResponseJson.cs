using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.SplitDay.AddSplitDay
{
    public class AddSplitDayResponseJson : BaseResponseJson
    {
        public UserDTO UserDto { get; set; } = new UserDTO();
    }
}