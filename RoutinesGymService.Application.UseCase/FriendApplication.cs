using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.AddNewUserFriend;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.DeleteFriend;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.GetAllUserFriends;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Application.Interface.Repository;

namespace RoutinesGymService.Application.UseCase
{
    public class FriendApplication : IFriendApplication
    {
        private readonly IFriendRepository _friendRepository;

        public FriendApplication(IFriendRepository friendRepository)
        {
            _friendRepository = friendRepository;
        }

        public async Task<AddNewUserFriendResponse> AddNewUserFriend(AddNewUserFriendRequest addNewUserFriendRequest)
        {
            return await _friendRepository.AddNewUserFriend(addNewUserFriendRequest);
        }

        public async Task<DeleteFriendResponse> DeleteFriend(DeleteFriendRequest deleteFriendRequest)
        {
            return await _friendRepository.DeleteFriend(deleteFriendRequest);
        }

        public async Task<GetAllUserFriendsResponse> GetAllUserFriends(GetAllUserFriendsRequest getAllUserFriendsRequest)
        {
            return await _friendRepository.GetAllUserFriends(getAllUserFriendsRequest);
        }
    }
}