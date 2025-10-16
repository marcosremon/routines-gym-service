using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetUsersByRole
{
    public class GetUsersByRoleResponse : BaseResponse
    {
        public List<UserDTO> UsersByRole { get; set; } = new List<UserDTO>();
    }
}
