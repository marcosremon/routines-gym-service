using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.AddNewUserFriend;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.DeleteFriend;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.GetAllUserFriends;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Infraestructure.Persistence.Context;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private readonly ApplicationDbContext _context;

        public FriendRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AddNewUserFriendResponse> AddNewUserFriend(AddNewUserFriendRequest addNewUserFriendRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<DeleteFriendResponse> DeleteFriend(DeleteFriendRequest deleteFriendRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<GetAllUserFriendsResponse> GetAllUserFriends(GetAllUserFriendsRequest getAllUserFriendsRequest)
        {
            throw new NotImplementedException();
        }
    }
}