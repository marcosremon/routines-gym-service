using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUserByEmail
{
    public class GetUserByEmailResponse : BaseResponse
    {
        public UserDTO UserDto { get; set; } = new UserDTO();
        public int RoutinesCount { get; set; } = 0;
        public int FriendsCount { get; set; } = 0;
        public bool LogoutAccount { get; set; }
    }
}