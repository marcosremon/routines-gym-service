using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.UpdateUser
{
    public class UpdateUserResponse : BaseResponse
    {
        public UserDTO UserDto { get; set; } = new UserDTO();
        public string NewToken { get; set; } = string.Empty;
    }
}