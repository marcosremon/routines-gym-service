using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetUsers
{
    public class GetUsersResponse : BaseResponse
    {
        public List<UserDTO> UsersDTO { get; set; } = new List<UserDTO>();
    }
}