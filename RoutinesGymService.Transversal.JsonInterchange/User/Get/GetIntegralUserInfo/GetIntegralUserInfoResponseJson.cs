using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.User.Get.GetIntegralUserInfo
{
    public class GetIntegralUserInfoResponseJson : BaseResponseJson
    {
        public UserDTO? UserDTO { get; set; }
    }
}