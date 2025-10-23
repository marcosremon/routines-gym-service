using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.AddNewUserFriend;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.AreFriends;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.DeleteFriend;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.GetAllUserFriends;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Application.Mapper;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Infraestructure.Persistence.Context;
using RoutinesGymService.Transversal.Common.Utils;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly GenericUtils _genericUtils;
        private readonly CacheUtils _cacheUtils;
        private readonly int _expiryMinutes;
        private readonly string _friendPrefix;

        public FriendRepository(ApplicationDbContext context, CacheUtils cacheUtils, GenericUtils genericUtils, IConfiguration configuration)
        {
            _cacheUtils = cacheUtils;
            _context = context;
            _genericUtils = genericUtils;
            _friendPrefix = configuration["CacheSettings:FriendPrefix"]!;
            _expiryMinutes = int.TryParse(configuration["CacheSettings:CacheExpiryMinutes"], out var m) ? m : 60;
        }

        #region Add new user friend
        public async Task<AddNewUserFriendResponse> AddNewUserFriend(AddNewUserFriendRequest addNewUserFriendRequest)
        {
            AddNewUserFriendResponse addNewUserFriendResponse = new AddNewUserFriendResponse();
            try
            {
                AreFriendsRequest areFriendsRequest = new AreFriendsRequest
                {
                    UserEmail = addNewUserFriendRequest.UserEmail,
                    FriendCode = addNewUserFriendRequest.FriendCode
                };

                AreFriendsResponse areFriendsResponse = await AreFriends(areFriendsRequest);
                if (areFriendsResponse.IsSuccess)
                {
                    addNewUserFriendResponse.IsSuccess = false;
                    addNewUserFriendResponse.Message = "This user is already your friend";
                }
                else
                {
                    UserFriend userFriend = new UserFriend
                    {
                        UserId = areFriendsResponse.UserId,
                        FriendId = areFriendsResponse.FriendId,
                    };

                    _genericUtils.ClearCache(_friendPrefix);

                    _context.UserFriends.Add(userFriend);
                    await _context.SaveChangesAsync();

                    userFriend = new UserFriend
                    {
                        UserId = areFriendsResponse.FriendId,
                        FriendId = areFriendsResponse.UserId,
                    };

                    _context.UserFriends.Add(userFriend);
                    await _context.SaveChangesAsync();

                    addNewUserFriendResponse.IsSuccess = true;
                    addNewUserFriendResponse.Message = "Friend added successfully";
                }
            }
            catch (Exception ex)
            {
                addNewUserFriendResponse.IsSuccess = false;
                addNewUserFriendResponse.Message = $"Unexpected error on FriendRepository -> AddNewUserFriend: {ex.Message}";
            }

            return addNewUserFriendResponse;
        }
        #endregion

        #region Delete friend
        public async Task<DeleteFriendResponse> DeleteFriend(DeleteFriendRequest deleteFriendRequest)
        {
            DeleteFriendResponse deleteFriendResponse = new DeleteFriendResponse();
            try
            {
                User? friendToDelete = await _context.Users.FirstOrDefaultAsync(u => u.Email == deleteFriendRequest.FriendEmail);
                if (friendToDelete == null)
                {
                    deleteFriendResponse.IsSuccess = false;
                    deleteFriendResponse.Message = "Friend not found";
                }
                else
                {
                    AreFriendsRequest areFriendsRequest = new AreFriendsRequest
                    {
                        UserEmail = deleteFriendRequest.UserEmail,
                        FriendCode = friendToDelete.FriendCode
                    };

                    AreFriendsResponse areFriendsResponse = await AreFriends(areFriendsRequest);
                    if (!areFriendsResponse.IsSuccess && areFriendsResponse.UserId == -1)
                    {
                        deleteFriendResponse.IsSuccess = false;
                        deleteFriendResponse.Message = areFriendsResponse.Message;
                    }
                    else if (!areFriendsResponse.IsSuccess)
                    {
                        deleteFriendResponse.IsSuccess = false;
                        deleteFriendResponse.Message = "You're not friends with this user";
                    }
                    else
                    {
                        UserFriend? userFriend = await _context.UserFriends.FirstOrDefaultAsync(f =>
                            f.UserId == areFriendsResponse.UserId &&
                            f.FriendId == areFriendsResponse.FriendId);

                        UserFriend? friendUser = await _context.UserFriends.FirstOrDefaultAsync(f =>
                            f.UserId == areFriendsResponse.FriendId &&
                            f.FriendId == areFriendsResponse.UserId);

                        if (userFriend != null && friendUser != null)
                        {
                            _genericUtils.ClearCache(_friendPrefix);

                            _context.UserFriends.Remove(userFriend);
                            _context.UserFriends.Remove(friendUser);
                            await _context.SaveChangesAsync();

                            deleteFriendResponse.IsSuccess = true;
                            deleteFriendResponse.Message = "Friend deleted successfully";
                        }
                        else
                        {
                            deleteFriendResponse.IsSuccess = false;
                            deleteFriendResponse.Message = "Friendship relationship not found";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                deleteFriendResponse.IsSuccess = false;
                deleteFriendResponse.Message = $"Unexpected error on FriendRepository -> DeleteFriend: {ex.Message}";
            }

            return deleteFriendResponse;
        }
        #endregion

        #region Get all user friends
        public async Task<GetAllUserFriendsResponse> GetAllUserFriends(GetAllUserFriendsRequest getAllUserFriendsRequest)
        {
            GetAllUserFriendsResponse getAllUserFriendsResponse = new GetAllUserFriendsResponse();
            try
            {
                string cacheKey = $"{_friendPrefix}GetAllUserFriends_{getAllUserFriendsRequest.UserEmail}";

                List<User>? cacheFriends = _cacheUtils.Get<List<User>>(cacheKey);
                if (cacheFriends != null)
                {
                    getAllUserFriendsResponse.IsSuccess = true;
                    getAllUserFriendsResponse.Message = "Friends retrieved successfully";
                    getAllUserFriendsResponse.Friends = cacheFriends.Select(f => UserMapper.UserToDto(f)).ToList();
                }
                else
                {
                    User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == getAllUserFriendsRequest.UserEmail);
                    if (user == null)
                    {
                        getAllUserFriendsResponse.IsSuccess = false;
                        getAllUserFriendsResponse.Message = "User not found";
                    }
                    else
                    {
                        List<long> friendsIds = await _context.UserFriends
                            .Where(uf => uf.UserId == user.UserId)
                            .Select(uf => uf.FriendId)
                            .ToListAsync();
                        if (friendsIds == null || !friendsIds.Any())
                        {
                            getAllUserFriendsResponse.IsSuccess = false;
                            getAllUserFriendsResponse.Message = "No friends found for this user";
                        }
                        else
                        {
                            List<User> friends = await _context.Users
                                .Where(u => friendsIds.Contains(u.UserId))
                                .ToListAsync();

                            getAllUserFriendsResponse.IsSuccess = true;
                            getAllUserFriendsResponse.Message = "Friends retrieved successfully";
                            getAllUserFriendsResponse.Friends = friends.Select(f => UserMapper.UserToDto(f)).ToList();

                            _cacheUtils.Set(cacheKey, friends, TimeSpan.FromMinutes(_expiryMinutes));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                getAllUserFriendsResponse.IsSuccess = false;
                getAllUserFriendsResponse.Message = $"Unexpected error on FriendRepository -> GetAllUserFriends: {ex.Message}";
            }

            return getAllUserFriendsResponse;
        }
        #endregion

        #region Auxiliary methods

        #region Are friends
        public async Task<AreFriendsResponse> AreFriends(AreFriendsRequest areFriendsRequest)
        {
            AreFriendsResponse areFriendsResponse = new AreFriendsResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == areFriendsRequest.UserEmail);
                if (user == null)
                {
                    areFriendsResponse.IsSuccess = false;
                    areFriendsResponse.Message = "User not found";
                }
                else
                {
                    User? friend = await _context.Users.FirstOrDefaultAsync(u => u.FriendCode == areFriendsRequest.FriendCode);
                    if (friend == null)
                    {
                        areFriendsResponse.IsSuccess = false;
                        areFriendsResponse.Message = "Friend not found";
                    }
                    else
                    {
                        bool areFriends = _context.UserFriends.Any(f => f.UserId == user.UserId && f.FriendId == friend.UserId);
                        
                        areFriendsResponse.UserId = user.UserId;
                        areFriendsResponse.FriendId = friend.UserId;
                        areFriendsResponse.IsSuccess = areFriends;
                        areFriendsResponse.Message = areFriends 
                            ? "This user is already your friend"
                            : "You're not friends";
                    }
                }
            }
            catch (Exception ex)
            {
                areFriendsResponse.IsSuccess = false;
                areFriendsResponse.Message = $"Unexpected error on FriendRepository -> AreFriends: {ex.Message}";
            }

            return areFriendsResponse;
        }
        #endregion

        #endregion
    }
}