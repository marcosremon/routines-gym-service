using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.User.Get.GetUserByEmail
{
    public class GetUserByEmailResponseJson : BaseResponseJson
    {
        public UserDTO UserDto { get; set; } = new UserDTO();
        public int RoutinesCount { get; set; } = 0;
        public int FriendsCount { get; set; } = 0;
        public bool LogoutAccount { get; set; } = false;
    }
}