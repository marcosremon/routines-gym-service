using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.User.Get.GetUserByEmail
{
    public class GetUserByEmailResponseJson : BaseResponseJson
    {
        public UserDTO? UserDTO { get; set; }
        public int RoutinesCount { get; set; }
        public int FriendsCount { get; set; }
        public bool LogoutAccount { get; set; } = false;
    }
}