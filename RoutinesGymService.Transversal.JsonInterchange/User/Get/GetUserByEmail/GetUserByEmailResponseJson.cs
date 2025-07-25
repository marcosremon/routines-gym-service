using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Transversal.JsonInterchange.User.Get.GetUserByEmail
{
    public class GetUserByEmailResponseJson : BaseRespoonseJson
    {
        public UserDTO? UserDTO { get; set; }
        public int RoutinesCount { get; set; }
        public int FriendsCount { get; set; }
    }
}