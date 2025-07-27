using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.AddNewUserFriend;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.DeleteFriend;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.GetAllUserFriends;

namespace RoutinesGymService.Application.Interface.Repository
{
    public interface IFriendRepository
    {
        Task<GetAllUserFriendsResponse> GetAllUserFriends(GetAllUserFriendsRequest getAllUserFriendsRequest);
        Task<AddNewUserFriendResponse> AddNewUserFriend(AddNewUserFriendRequest addNewUserFriendRequest);
        Task<DeleteFriendResponse> DeleteFriend(DeleteFriendRequest deleteFriendRequest);
    }
}