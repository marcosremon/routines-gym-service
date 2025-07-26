using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Transversal.JsonInterchange.User.Create.CreateGoogleUser
{
    public class CreateGoogleUserResponseJson : BaseResponseJson
    {
        public UserDTO? UserDTO { get; set; }
    }
}