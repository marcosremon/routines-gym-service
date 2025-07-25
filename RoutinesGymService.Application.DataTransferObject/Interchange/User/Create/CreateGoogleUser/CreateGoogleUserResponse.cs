using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateGoogleUser
{
    public class CreateGoogleUserResponse : BaseResponse
    {
        public UserDTO? UserDTO { get; set; }
    }
}