using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetIntegralUserInfo
{
    public class GetIntegralUserInfoResponse : BaseResponse
    {
        public UserDTO? UserDTO { get; set; }
    }
}