using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUsers
{
    public class GetUsersResponse : BaseResponse
    {
        public List<UserDTO>? UsersDTO { get; set; }
    }
}