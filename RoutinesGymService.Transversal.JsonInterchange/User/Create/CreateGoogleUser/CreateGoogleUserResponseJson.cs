using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Transversal.JsonInterchange.User.Create.CreateGoogleUser
{
    public class CreateGoogleUserResponseJson : BaseRespoonseJson
    {
        public UserDTO? UserDTO { get; set; }
    }
}