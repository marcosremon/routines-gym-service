using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.AddNewUserFriend;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.DeleteFriend;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.GetAllUserFriends;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Application.Mapper;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Infraestructure.Persistence.Context;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly GenericUtils _genericUtils;
        private readonly CacheService _cache;
        private readonly int _expiryMinutes;
        private readonly string _friendPrefix;

        public FriendRepository(ApplicationDbContext context, CacheService cache, GenericUtils genericUtils, IConfiguration configuration)
        {
            _cache = cache;
            _context = context;
            _genericUtils = genericUtils;
            _friendPrefix = configuration["CacheSettings:FriendPrefix"]!;
            _expiryMinutes = int.TryParse(configuration["CacheSettings:CacheExpiryMinutes"], out var m) ? m : 60;
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
                        if (_context.UserFriends.Any(f => f.UserId == user.UserId && f.FriendId == friend.UserId))
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

                            _genericUtils.ClearCache(_friendPrefix);

                            await _context.SaveChangesAsync();

                            addNewUserFriendResponse.IsSuccess = true;
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
            DeleteFriendResponse deleteFriendResponse = new DeleteFriendResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == deleteFriendRequest.UserEmail);
                if (user == null)
                {
                    deleteFriendResponse.IsSuccess = false;
                    deleteFriendResponse.Message = "User not found";
                }
                else
                {
                    User? friend = await _context.Users.FirstOrDefaultAsync(u => u.Email == deleteFriendRequest.FriendEmail);
                    if (friend == null)
                    {
                        deleteFriendResponse.IsSuccess = false;
                        deleteFriendResponse.Message = "Friend not found";
                    }
                    else
                    {
                        UserFriend? userFriend = _context.UserFriends.FirstOrDefault(f => f.UserId == user.UserId && f.FriendId == friend.UserId);
                        if (userFriend == null)
                        {
                            deleteFriendResponse.IsSuccess = false;
                            deleteFriendResponse.Message = "This user is already your friend";
                        }
                        else
                        {   
                            _context.UserFriends.Remove(userFriend);
                            _genericUtils.ClearCache(_friendPrefix);

                            await _context.SaveChangesAsync();

                            deleteFriendResponse.IsSuccess = true;
                            deleteFriendResponse.Message = "Friend added successfully";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                deleteFriendResponse.IsSuccess = false;
                deleteFriendResponse.Message = $"Unexpected error on FriendRepository -> DeleteFriendResponse: {ex.Message}";
            }

            return deleteFriendResponse;
        }

        public async Task<GetAllUserFriendsResponse> GetAllUserFriends(GetAllUserFriendsRequest getAllUserFriendsRequest)
        {
            GetAllUserFriendsResponse getAllUserFriendsResponse = new GetAllUserFriendsResponse();
            try
            {
                string cacheKey = $"{_friendPrefix}_GetAllUserFriends_{getAllUserFriendsRequest.UserEmail}";

                List<User>? cacheFriends = _cache.Get<List<User>>(cacheKey);
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
                        if (friendsIds == null || friendsIds.Count == 0)
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

                            _cache.Set(cacheKey, friends, TimeSpan.FromMinutes(_expiryMinutes));
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
    }
}