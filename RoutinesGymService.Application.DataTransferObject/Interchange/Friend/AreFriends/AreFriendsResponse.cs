using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Friend.AreFriends
{
    public class AreFriendsResponse : BaseResponse
    {
        public long UserId { get; set; }
        public long FriendId { get; set; }
    }
}