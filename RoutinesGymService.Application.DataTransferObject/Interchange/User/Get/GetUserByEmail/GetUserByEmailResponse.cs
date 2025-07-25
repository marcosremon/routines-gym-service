using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUserByEmail
{
    public class GetUserByEmailResponse : BaseResponse
    {
        public UserDTO? UserDTO { get; set; }
        public int RoutinesCount { get; set; }
        public int FriendsCount { get; set; }
    }
}