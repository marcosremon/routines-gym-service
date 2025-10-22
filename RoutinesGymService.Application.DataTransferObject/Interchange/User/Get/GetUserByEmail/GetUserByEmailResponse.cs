using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUserByEmail
{
    public class GetUserByEmailResponse : BaseResponse
    {
        public UserDTO UserDTO { get; set; } = new UserDTO();
        public int RoutinesCount { get; set; }
        public int FriendsCount { get; set; }
        public bool LogoutAccount { get; set; }
    }
}