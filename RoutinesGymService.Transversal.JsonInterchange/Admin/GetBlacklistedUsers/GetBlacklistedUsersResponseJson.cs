using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Admin.GetBlacklistedUsers
{
    public class GetBlacklistedUsersResponseJson : BaseResponseJson
    {
        public List<UserDTO> BlackListUsers { get; set; } = new List<UserDTO>();
    }
}