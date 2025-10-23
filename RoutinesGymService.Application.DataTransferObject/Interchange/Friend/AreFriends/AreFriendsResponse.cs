using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Friend.AreFriends
{
    public class AreFriendsResponse : BaseResponse
    {
        public long UserId { get; set; } = -1;
        public long FriendId { get; set; } = -1;
    }
}