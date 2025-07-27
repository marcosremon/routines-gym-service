using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.UpdateUser
{
    public class UpdateUserResponse : BaseResponse
    {
        public UserDTO? UserDTO { get; set; }
    }
}