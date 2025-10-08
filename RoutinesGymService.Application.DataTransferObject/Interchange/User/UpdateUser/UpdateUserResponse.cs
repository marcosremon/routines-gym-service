using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.UpdateUser
{
    public class UpdateUserResponse : BaseResponse
    {
        public UserDTO? UserDTO { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}