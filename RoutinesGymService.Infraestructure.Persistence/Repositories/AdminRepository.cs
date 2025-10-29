using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.ChangeUserPassword;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.ChangeUserRole;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetBlacklistedUsers;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetIntegralUserInfo;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetIntegralUsers;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetUsers;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetUsersByRole;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.RemoveUserFromBlackList;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.AddUserToBlackList;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Application.Mapper;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Infraestructure.Persistence.Context;
using RoutinesGymService.Transversal.Common.Utils;
using RoutinesGymService.Transversal.Security.Utils;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly GenericUtils _genericUtils;
        private readonly CacheUtils _cacheUtils;
        private readonly PasswordUtils _passwordUtils;
        private readonly string _userPrefix;
        private readonly int _expiryMinutes;

        public AdminRepository(ApplicationDbContext context, PasswordUtils passwordUtils, CacheUtils cacheUtils, GenericUtils genericUtils, IConfiguration configuration)
        {
            _context = context;
            _genericUtils = genericUtils;
            _passwordUtils = passwordUtils;
            _cacheUtils = cacheUtils;
            _userPrefix = configuration["CacheSettings:UserPrefix"]!;
            _expiryMinutes = int.TryParse(configuration["CacheSettings:CacheExpiryMinutes"], out var m) ? m : 60;
        }

        #region Get users
        public async Task<GetUsersResponse> GetUsers()
        {
            GetUsersResponse getUsersResponse = new GetUsersResponse();
            try
            {
                string cacheKey = $"{_userPrefix}GetUsers_All";

                List<User>? cacheUsers = _cacheUtils.Get<List<User>>(cacheKey);
                if (cacheUsers != null)
                {
                    if (!cacheUsers.Any())
                    {
                        getUsersResponse.Message = "No users found";
                        getUsersResponse.IsSuccess = false;
                    }
                    else
                    {
                        getUsersResponse.IsSuccess = true;
                        getUsersResponse.Message = "Users found successfully";
                        getUsersResponse.UsersDto = cacheUsers.Select(UserMapper.UserToDto).ToList();
                    }
                }
                else
                {
                    List<User> users = await _context.Users.ToListAsync();
                    getUsersResponse.IsSuccess = true;
                    getUsersResponse.Message = "Users found successfully";
                    getUsersResponse.UsersDto = users.Select(UserMapper.UserToDto).ToList();

                    _cacheUtils.Set(cacheKey, users, TimeSpan.FromMinutes(_expiryMinutes));
                }
            }
            catch (Exception ex)
            {
                getUsersResponse.Message = $"unexpected error on UserRepository -> GetUsers: {ex.Message}";
                getUsersResponse.IsSuccess = false;
            }

            return getUsersResponse;
        }
        #endregion

        #region Add user to black list
        public async Task<AddUserToBlackListResponse> AddUserToBlackList(AddUserToBlackListRequest addUserToBlackListRequest)
        {
            AddUserToBlackListResponse addUserToBlackListResponse = new AddUserToBlackListResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == addUserToBlackListRequest.UserId);
                if (user == null)
                {
                    addUserToBlackListResponse.IsSuccess = false;
                    addUserToBlackListResponse.Message = $"User not found with the id {addUserToBlackListRequest.UserId}";
                }
                else if (addUserToBlackListRequest.SerialNumber != user.SerialNumber)
                {
                    addUserToBlackListResponse.IsSuccess = false;
                    addUserToBlackListResponse.Message = $"That serial number does not match the user with id: {addUserToBlackListRequest.UserId}";
                }
                else
                {
                    bool isOnBlackList = await _context.BlackList.AnyAsync(bl => 
                            bl.SerialNumber == addUserToBlackListRequest.SerialNumber && 
                            bl.UserId == addUserToBlackListRequest.UserId);
                    if (isOnBlackList)
                    {
                        addUserToBlackListResponse.IsSuccess = false;
                        addUserToBlackListResponse.Message = $"The user is already on the blacklist";
                    }
                    else
                    {
                        BlackList blackList = new BlackList
                        {
                            SerialNumber = addUserToBlackListRequest.SerialNumber,
                            UserId = addUserToBlackListRequest.UserId,
                            Description = addUserToBlackListRequest.Description,
                        };

                        _genericUtils.ClearCache(_userPrefix);

                        await _context.BlackList.AddAsync(blackList);
                        await _context.SaveChangesAsync();

                        addUserToBlackListResponse.IsSuccess = true;
                        addUserToBlackListResponse.Message = $"User with ID: {addUserToBlackListRequest.UserId} and Serial Number: {addUserToBlackListRequest.SerialNumber} added successfuly";
                    }
                }
            }
            catch (Exception ex)
            {
                addUserToBlackListResponse.IsSuccess = false;
                addUserToBlackListResponse.Message = $"unexpected error on UserRepository -> AddUserToBlackList: {ex.Message}";
            }

            return addUserToBlackListResponse;
        }
        #endregion

        #region Get integral user info
        public async Task<GetIntegralUserInfoResponse> GetIntegralUserInfo(GetIntegralUserInfoRequest getIntegralUserInfoRequest)
        {
            GetIntegralUserInfoResponse getIntegralUserInfoResponse = new GetIntegralUserInfoResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == getIntegralUserInfoRequest.UserId);
                if (user == null)
                {
                    getIntegralUserInfoResponse.IsSuccess = false;
                    getIntegralUserInfoResponse.UserDto = new UserDTO();
                    getIntegralUserInfoResponse.Message = $"User not found";
                }
                else
                {
                    UserDTO userDto = UserMapper.UserToDto(user);
                    userDto.Password = PasswordUtils.DecryptPasswordWithMasterKey(user.Password, getIntegralUserInfoRequest.MasterKey);

                    getIntegralUserInfoResponse.IsSuccess = true;
                    getIntegralUserInfoResponse.UserDto = userDto;
                    getIntegralUserInfoResponse.Message = $"User found";
                }
            }
            catch (Exception ex)
            {
                getIntegralUserInfoResponse.IsSuccess = false;
                getIntegralUserInfoResponse.Message = $"unexpected error on UserRepository -> GetIntegralUserInfo: {ex.Message}";
            }

            return getIntegralUserInfoResponse;
        }
        #endregion

        #region Get integral users
        public async Task<GetIntegralUsersResponse> GetIntegralUsers(GetIntegralUsersRequest getIntegralUsersRequest)
        {
            GetIntegralUsersResponse getIntegralUsersResponse = new GetIntegralUsersResponse();
            try
            {
                List<User> users = await _context.Users.ToListAsync();
                if (!users.Any())
                {
                    getIntegralUsersResponse.IsSuccess = false;
                    getIntegralUsersResponse.Message = $"Users not found";
                }
                else
                {
                    List<UserDTO> usersDto = users.Select(u =>
                    {
                        UserDTO user = UserMapper.UserToDto(u);
                        user.Password = PasswordUtils.DecryptPasswordWithMasterKey(u.Password, getIntegralUsersRequest.MasterKey);
                        return user;
                    }).ToList();

                    getIntegralUsersResponse.IsSuccess = true;
                    getIntegralUsersResponse.Message = $"Users found";
                    getIntegralUsersResponse.UsersDto = usersDto;
                }
            }
            catch (Exception ex)
            {
                getIntegralUsersResponse.IsSuccess = false;
                getIntegralUsersResponse.Message = $"unexpected error on UserRepository -> GetIntegralUserInfo: {ex.Message}";
            }

            return getIntegralUsersResponse;
        }
        #endregion

        #region Change user role
        public async Task<ChangeUserRoleResponse> ChangeUserRole(ChangeUserRoleRequest changeUserRoleRequest)
        {
            ChangeUserRoleResponse changeUserRoleResponse = new ChangeUserRoleResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == changeUserRoleRequest.UserEmail);
                if (user == null)
                {
                    changeUserRoleResponse.IsSuccess = false;
                    changeUserRoleResponse.Message = "User not found";
                }
                else
                {
                    if (changeUserRoleRequest.OldRole == changeUserRoleRequest.NewRole)
                    {
                        changeUserRoleResponse.IsSuccess = false;
                        changeUserRoleResponse.Message = "The old role is the same as the new role";
                    }
                    else
                    {
                        if (user.RoleString.ToLower() == changeUserRoleRequest.NewRole.ToLower())
                        {
                            changeUserRoleResponse.IsSuccess = false;
                            changeUserRoleResponse.Message = $"The user already has the role {changeUserRoleRequest.NewRole}";
                        }
                        else
                        {
                            user.RoleString = changeUserRoleRequest.NewRole;
                            await _context.SaveChangesAsync();

                            _genericUtils.ClearCache(_userPrefix);

                            changeUserRoleResponse.IsSuccess = true;
                            changeUserRoleResponse.Message = $"User role updated successfully to {changeUserRoleRequest.NewRole}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                changeUserRoleResponse.IsSuccess = false;
                changeUserRoleResponse.Message = $"unexpected error on UserRepository -> ChangeUserRole: {ex.Message}";
            }

            return changeUserRoleResponse;
        }
        #endregion

        #region Get blacklisted users
        public async Task<GetBlacklistedUsersResponse> GetBlacklistedUsers(GetBlacklistedUsersRequest getBlacklistedUsersRequest)
        {
            GetBlacklistedUsersResponse getBlacklistedUsersResponse = new GetBlacklistedUsersResponse();
            try
            {
                List<UserDTO> blacklistedUsers = await _context.Users
                    .Join(_context.BlackList,
                        user => user.UserId,
                        black => black.UserId,
                        (user, black) => new UserDTO
                        {
                            Dni = user.Dni,
                            Username = user.Username,
                            Surname = user.Surname,
                            Email = user.Email,
                            FriendCode = user.FriendCode,
                            Password = PasswordUtils.DecryptPasswordWithMasterKey(user.Password, getBlacklistedUsersRequest.MasterKey),
                            Role = GenericUtils.ChangeIntToEnumOnRole(user.Role),
                            InscriptionDate = user.InscriptionDate.ToString(),
                        })
                    .ToListAsync();

                if (!blacklistedUsers.Any())
                {
                    getBlacklistedUsersResponse.IsSuccess = false;
                    getBlacklistedUsersResponse.Message = "No blacklisted users found";
                }
                else
                {
                    getBlacklistedUsersResponse.IsSuccess = true;
                    getBlacklistedUsersResponse.Message = "Blacklisted users found successfully";
                    getBlacklistedUsersResponse.BlackListUsers = blacklistedUsers;
                }
            }
            catch (Exception ex)
            {
                getBlacklistedUsersResponse.IsSuccess = false;
                getBlacklistedUsersResponse.Message = $"unexpected error on UserRepository -> GetBlacklistedUsers: {ex.Message}";
            }

            return getBlacklistedUsersResponse;
        }
        #endregion

        #region Remove user from black list
        public async Task<RemoveUserFromBlackListResponse> RemoveUserFromBlackList(RemoveUserFromBlackListRequest removeUserFromBlackListRequest)
        {
            RemoveUserFromBlackListResponse removeUserFromBlackListResponse = new RemoveUserFromBlackListResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == removeUserFromBlackListRequest.UserEmail);
                if (user == null)
                {
                    removeUserFromBlackListResponse.IsSuccess = false;
                    removeUserFromBlackListResponse.Message = "User not found";
                }
                else
                {
                    BlackList? blackListUser = await _context.BlackList.FirstOrDefaultAsync(bl => bl.UserId == user.UserId && bl.SerialNumber == user.SerialNumber);
                    if (blackListUser == null)
                    {
                        removeUserFromBlackListResponse.IsSuccess = false;
                        removeUserFromBlackListResponse.Message = "User is not on black list";
                    }
                    else
                    {
                        _context.BlackList.Remove(blackListUser);
                        await _context.SaveChangesAsync();

                        _genericUtils.ClearCache(_userPrefix);

                        removeUserFromBlackListResponse.IsSuccess = true;
                        removeUserFromBlackListResponse.Message = "User removed from black list successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                removeUserFromBlackListResponse.IsSuccess = false;
                removeUserFromBlackListResponse.Message = $"unexpected error on UserRepository -> RemoveUserFromBlackList: {ex.Message}";
            }

            return removeUserFromBlackListResponse;
        }
        #endregion

        #region Get users by role
        public async Task<GetUsersByRoleResponse> GetUsersByRole(GetUsersByRoleRequest getUsersByRoleRequest)
        {
            GetUsersByRoleResponse getUsersByRoleResponse = new GetUsersByRoleResponse();
            try
            {
                List<User> users = await _context.Users
                    .Where(u => u.RoleString.ToLower() == getUsersByRoleRequest.Role.ToLower())
                    .ToListAsync();
                if (!users.Any())
                {
                    getUsersByRoleResponse.IsSuccess = false;
                    getUsersByRoleResponse.Message = $"No users found with the role {getUsersByRoleRequest.Role}";
                }
                else
                {
                    getUsersByRoleResponse.IsSuccess = true;
                    getUsersByRoleResponse.Message = $"Users found with the role {getUsersByRoleRequest.Role}";
                    getUsersByRoleResponse.UsersByRole = users.Select(UserMapper.UserToDto).ToList();
                }
            }
            catch (Exception ex)
            {
                getUsersByRoleResponse.IsSuccess = false;
                getUsersByRoleResponse.Message = $"unexpected error on UserRepository -> GetUsersByRole: {ex.Message}";
            }

            return getUsersByRoleResponse;
        }
        #endregion

        #region Change user password    
        public async Task<ChangeUserPasswordResponse> ChangeUserPassword(ChangeUserPasswordRequest changeUserPasswordRequest)
        {
            ChangeUserPasswordResponse changeUserPasswordResponse = new ChangeUserPasswordResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == changeUserPasswordRequest.UserEmail);
                if (user == null)
                {
                    changeUserPasswordResponse.IsSuccess = false;
                    changeUserPasswordResponse.Message = $"User not found";
                }
                else
                {
                    byte[] newUserPassword = _passwordUtils.PasswordEncoder(changeUserPasswordRequest.NewPassword);
                    user.Password = newUserPassword;

                    await _context.SaveChangesAsync();

                    _genericUtils.ClearCache(_userPrefix);

                    changeUserPasswordResponse.IsSuccess = true;
                    changeUserPasswordResponse.Message = $"User password updated successfully";
                }
            }
            catch (Exception ex)
            {
                changeUserPasswordResponse.IsSuccess = false;
                changeUserPasswordResponse.Message = $"unexpected error on UserRepository -> ChangeUserPassword: {ex.Message}";
            }

            return changeUserPasswordResponse;
        }
        #endregion
    }
}