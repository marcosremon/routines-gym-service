using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Admin.GetUsersByRole
{
    public class GetUsersByRoleResponseJson : BaseResponseJson
    {
        public List<UserDTO> UsersByRole { get; set; } = new List<UserDTO>();
    }
}