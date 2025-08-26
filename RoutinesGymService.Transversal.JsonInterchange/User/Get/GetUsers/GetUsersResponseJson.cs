using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.User.Get.GetUsers
{
    public class GetUsersResponseJson : BaseResponseJson
    {
        public List<UserDTO>? UsersDTO { get; set; }
    }
}