using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Friend.GetAllUserFriends
{
    public class GetAllUserFriendsResponseJson : BaseResponseJson
    {
        public List<UserDTO> Friends { get; set; } = new List<UserDTO>();
    }
}