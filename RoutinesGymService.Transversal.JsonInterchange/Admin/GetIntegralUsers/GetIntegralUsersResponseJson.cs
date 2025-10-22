using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Admin.GetIntegralUsers
{
    public class GetIntegralUsersResponseJson : BaseResponseJson
    {
        public List<UserDTO> UsersDto { get; set; } = new List<UserDTO>();
    }
}