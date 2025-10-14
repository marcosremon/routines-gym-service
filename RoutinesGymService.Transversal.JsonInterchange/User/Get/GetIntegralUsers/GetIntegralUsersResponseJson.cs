using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.User.Get.GetIntegralUsers
{
    public class GetIntegralUsersResponseJson : BaseResponseJson
    {
        public List<UserDTO> UsersDTO { get; set; } = new List<UserDTO>();
    }
}