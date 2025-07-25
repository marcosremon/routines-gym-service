using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateUser
{
    public class CreateUserResponse : BaseResponse
    {
        public UserDTO? UserDTO { get; set; }
    }
}