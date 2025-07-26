using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Friend.GetAllUserFriends
{
    public class GetAllUserFriendsResponse : BaseResponse
    {
        public List<UserDTO> Friends { get; set; } = new List<UserDTO>();
    }
}