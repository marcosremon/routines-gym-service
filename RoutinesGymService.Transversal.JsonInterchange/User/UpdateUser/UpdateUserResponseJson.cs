using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.User.UpdateUser
{
    public class UpdateUserResponseJson : BaseResponseJson
    {
        public UserDTO userDto { get; set; } = new UserDTO();
        public string NewToken { get; set; } = string.Empty;
    }
}