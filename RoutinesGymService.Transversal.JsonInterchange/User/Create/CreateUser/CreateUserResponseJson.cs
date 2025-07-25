using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Transversal.JsonInterchange.User.Create.CreateUser
{
    public class CreateUserResponseJson : BaseRespoonseJson
    {
        public UserDTO? UserDTO { get; set; }
    }
}