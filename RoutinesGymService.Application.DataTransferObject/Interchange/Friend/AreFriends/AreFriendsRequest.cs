namespace RoutinesGymService.Application.DataTransferObject.Interchange.Friend.AreFriends
{
    public class AreFriendsRequest
    {
        public string UserEmail { get; set; } = string.Empty;
        public string FriendCode { get; set; } = string.Empty;
    }
}