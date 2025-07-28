using Microsoft.EntityFrameworkCore;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.AddNewUserFriend;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.DeleteFriend;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.GetAllUserFriends;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Domain.Model.Entities;
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
            AddNewUserFriendResponse addNewUserFriendResponse = new AddNewUserFriendResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == addNewUserFriendRequest.UserEmail);
                if (user == null)
                {
                    addNewUserFriendResponse.IsSuccess = false;
                    addNewUserFriendResponse.Message = "User not found";
                }
                else
                {
                    User? friend = await _context.Users.FirstOrDefaultAsync(u => u.FriendCode == addNewUserFriendRequest.FriendCode);
                    if (friend == null)
                    {
                        addNewUserFriendResponse.IsSuccess = false;
                        addNewUserFriendResponse.Message = "Friend not found";
                    }
                    else
                    {
                        if (!_context.UserFriends.Any(f => f.UserId == user.UserId && f.FriendId == friend.UserId))
                        {
                            addNewUserFriendResponse.IsSuccess = false;
                            addNewUserFriendResponse.Message = "This user is already your friend";
                        }
                        else
                        {
                            UserFriend userFriend = new UserFriend
                            {
                                UserId = user.UserId,
                                FriendId = friend.UserId,
                            };

                            _context.UserFriends.Add(userFriend);
                            await _context.SaveChangesAsync();

                            addNewUserFriendResponse.IsSuccess = true;
                            addNewUserFriendResponse.FriendId = userFriend.FriendId;
                            addNewUserFriendResponse.Message = "Friend added successfully";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                addNewUserFriendResponse.IsSuccess = false;
                addNewUserFriendResponse.Message = $"Unexpected error on FriendRepository -> AddNewUserFriend: {ex.Message}";
            }
        
            return addNewUserFriendResponse;
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