using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetBlacklistedUsers
{
    public class GetBlacklistedUsersResponse : BaseResponse
    {
        public List<UserDTO> BlackListUsers { get; set; } = new List<UserDTO>();
    }
}