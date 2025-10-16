using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetIntegralUsers
{
    public class GetIntegralUsersResponse : BaseResponse
    {
        public List<UserDTO> UsersDto { get; set; } = new List<UserDTO>();
    }
}