using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RoutinesGymApp.Domain.Entities;
using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Check.CheckUserExistence;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.AddUserToBlackList;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.ChangePasswordWithPasswordAndEmail;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateGenericUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateGoogleUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateNewPassword;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.DeleteUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetIntegralUserInfo;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetIntegralUsers;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUserByEmail;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUserProfileDetails;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUsers;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.UpdateUser;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Application.Mapper;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Domain.Model.Enums;
using RoutinesGymService.Infraestructure.Persistence.Context;
using RoutinesGymService.Transversal.Common.Utils;
using RoutinesGymService.Transversal.Security;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly GenericUtils _genericUtils;
        private readonly CacheUtils _cacheUtils;
        private readonly PasswordUtils _passwordUtils;
        private readonly string _userPrefix;
        private readonly string _stepPrefix;
        private readonly string _routinePrefix;
        private readonly string _friendPrefix;
        private readonly string _authPrefix;
        private readonly string _exercisePrefix;
        private readonly int _expiryMinutes;

        public UserRepository(ApplicationDbContext context, CacheUtils cacheUtils, GenericUtils genericUtils, PasswordUtils passwordUtils, IConfiguration configuration)
        {
            _context = context;
            _genericUtils = genericUtils;
            _cacheUtils = cacheUtils;
            _passwordUtils = passwordUtils;
            _userPrefix = configuration["CacheSettings:UserPrefix"]!;
            _stepPrefix = configuration["CacheSettings:StepPrefix"]!;
            _routinePrefix = configuration["CacheSettings:RoutinePrefix"]!;
            _friendPrefix = configuration["CacheSettings:FriendPrefix"]!;
            _authPrefix = configuration["CacheSettings:AuthPrefix"]!;
            _exercisePrefix = configuration["CacheSettings:ExercisePrefix"]!;
            _expiryMinutes = int.TryParse(configuration["CacheSettings:CacheExpiryMinutes"], out var m) ? m : 60;
        }

        public async Task<GetUserByEmailResponse> GetUserByEmail(GetUserByEmailRequest getUserByEmailRequest)
        {
            GetUserByEmailResponse getUserByEmailResponse = new GetUserByEmailResponse();
            try
            {
                string cacheKey = $"{_userPrefix}GetUserByEmail_{getUserByEmailRequest.Email}";

                User? cachedUser = _cacheUtils.Get<User>(cacheKey);
                if (cachedUser != null)
                {
                    getUserByEmailResponse.IsSuccess = true;
                    getUserByEmailResponse.Message = "User found successfully";
                    getUserByEmailResponse.RoutinesCount = cachedUser.Routines.Count;
                    getUserByEmailResponse.FriendsCount = await _context.UserFriends.CountAsync(u => u.UserId == cachedUser.UserId);
                    getUserByEmailResponse.UserDTO = UserMapper.UserToDto(cachedUser);
                }
                else
                {
                    User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == getUserByEmailRequest.Email);
                    if (user == null)
                    {
                        getUserByEmailResponse.IsSuccess = false;
                        getUserByEmailResponse.Message = "User not found with the provided email";
                    }
                    else
                    {
                        bool isOnBlackList = await _context.BlackList.AnyAsync(bl => bl.SerialNumber == user.SerialNumber);
                        if (isOnBlackList)
                        {
                            getUserByEmailResponse.IsSuccess = true;
                            getUserByEmailResponse.LogoutAccount = true;
                            getUserByEmailResponse.Message = "You are in Black List 💀";
                        }
                        else
                        {
                            getUserByEmailResponse.LogoutAccount = false;
                            getUserByEmailResponse.IsSuccess = true;
                            getUserByEmailResponse.Message = "User found successfully";
                            getUserByEmailResponse.RoutinesCount = user.Routines.Count;
                            getUserByEmailResponse.FriendsCount = await _context.UserFriends.CountAsync(u => u.UserId == user.UserId);
                            getUserByEmailResponse.UserDTO = UserMapper.UserToDto(user);

                            _cacheUtils.Set(cacheKey, user, TimeSpan.FromMinutes(_expiryMinutes));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                getUserByEmailResponse.Message = $"unexpected error on UserRepository -> GetUserByEmail: {ex.Message}";
                getUserByEmailResponse.IsSuccess = false;
            }

            return getUserByEmailResponse;
        }

        public async Task<GetUsersResponse> GetUsers()
        {
            GetUsersResponse getUsersResponse = new GetUsersResponse();
            try
            {
                string cacheKey = $"{_userPrefix}GetUsers_All";

                List<User>? cacheUsers = _cacheUtils.Get<List<User>>(cacheKey);
                if (cacheUsers != null)
                {
                    if (cacheUsers.Count == 0)
                    {
                        getUsersResponse.Message = "No users found";
                        getUsersResponse.IsSuccess = false;
                    }
                    else
                    {
                        getUsersResponse.IsSuccess = true;
                        getUsersResponse.Message = "Users found successfully";
                        getUsersResponse.UsersDTO = cacheUsers.Select(UserMapper.UserToDto).ToList();
                    }
                }
                else
                {
                    List<User> users = await _context.Users.ToListAsync();
                    getUsersResponse.IsSuccess = true;
                    getUsersResponse.Message = "Users found successfully";
                    getUsersResponse.UsersDTO = users.Select(UserMapper.UserToDto).ToList();

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

        public async Task<CreateUserResponse> CreateUser(CreateGenericUserRequest createGenericUserRequest)
        {
            CreateUserResponse createUserResponse = new CreateUserResponse();
            try
            {
                bool isOnBlackList = await _context.BlackList.AnyAsync(bl => bl.SerialNumber == createGenericUserRequest.SerialNumber);
                if (isOnBlackList)
                {
                    createUserResponse.IsSuccess = false;
                    createUserResponse.Message = "You are in Black List 💀";
                }
                else if (!GenericUtils.IsDniValid(createGenericUserRequest.Dni!))
                {
                    createUserResponse.IsSuccess = false;
                    createUserResponse.Message = "The dni is not valid you need eight numbers and a capital letter";
                }
                else if (!MailUtils.IsEmailValid(createGenericUserRequest.Email!))
                {
                    createUserResponse.IsSuccess = false;
                    createUserResponse.Message = "Invalid email format example (example@gmail.com)";
                }
                else
                {
                    User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == createGenericUserRequest.Email);
                    if (user != null)
                    {
                        createUserResponse.IsSuccess = false;
                        createUserResponse.Message = "User already exists with the provided email";
                    }
                    else
                    {
                        user = await _context.Users.FirstOrDefaultAsync(u => u.Dni == createGenericUserRequest.Dni);
                        if (user != null)
                        {
                            createUserResponse.IsSuccess = false;
                            createUserResponse.Message = "User already exists with the provided DNI";
                        }
                        else
                        {
                            user = await _context.Users.FirstOrDefaultAsync(u => u.Dni == createGenericUserRequest.Dni && u.Email == createGenericUserRequest.Email);
                            if (user == null)
                            {
                                createUserResponse.IsSuccess = false;
                                createUserResponse.Message = "The email entered does not match the user's email address with the ID number entered";
                            }
                            else
                            {
                                if (createGenericUserRequest.Password != createGenericUserRequest.ConfirmPassword)
                                {
                                    createUserResponse.IsSuccess = false;
                                    createUserResponse.Message = "Password and confirm password do not match";
                                }
                                else
                                {
                                    if (!PasswordUtils.IsPasswordValid(createGenericUserRequest.Password!))
                                    {
                                        createUserResponse.IsSuccess = false;
                                        createUserResponse.Message = "Password does not meet the required criteria you need: eight characters with one upper case, one lower case, one number and one special character.";
                                    }
                                    else
                                    {
                                        string friendCode = string.Empty;
                                        do
                                        {
                                            friendCode = GenericUtils.CreateFriendCode(8);
                                        }
                                        while (await _context.Users.AnyAsync(u => u.FriendCode == friendCode));

                                        if (createGenericUserRequest.SerialNumber == "UNKNOWN_IOS" ||
                                            createGenericUserRequest.SerialNumber == "UNKNOWN" ||
                                            createGenericUserRequest.SerialNumber.Contains("ERROR_"))
                                        {
                                            string newSerial = string.Empty;
                                            do
                                            {
                                                newSerial = Guid.NewGuid().ToString();
                                            }
                                            while (await _context.Users.AnyAsync(u => u.SerialNumber == newSerial));

                                            createGenericUserRequest.SerialNumber = newSerial;
                                        }

                                        User newUser = new User
                                        {
                                            Dni = createGenericUserRequest.Dni!,
                                            Username = createGenericUserRequest.Username!,
                                            Surname = createGenericUserRequest.Surname ?? "",
                                            Email = createGenericUserRequest.Email!.ToLower(),
                                            FriendCode = friendCode,
                                            SerialNumber = createGenericUserRequest.SerialNumber!,
                                            Password = _passwordUtils.PasswordEncoder(createGenericUserRequest.Password!),
                                            Role = GenericUtils.ChangeEnumToIntOnRole(createGenericUserRequest.Role),
                                            RoleString = createGenericUserRequest.Role.ToString().ToLower(),
                                            InscriptionDate = DateTime.UtcNow
                                        };

                                        _genericUtils.ClearCache(_userPrefix);

                                        await _context.Users.AddAsync(newUser);
                                        await _context.SaveChangesAsync();

                                        createUserResponse.IsSuccess = true;
                                        createUserResponse.Message = "User created successfully";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                createUserResponse.Message = $"unexpected error on UserRepository -> CreateUser: {ex.Message}";
                createUserResponse.IsSuccess = false;
            }

            return createUserResponse;
        }

        public async Task<CreateGoogleUserResponse> CreateGoogleUser(CreateGenericUserRequest createGenericUserRequest)
        {
            CreateGoogleUserResponse createGoogleUserResponse = new CreateGoogleUserResponse();
            try
            {
                bool isOnBlackList = await _context.BlackList.AnyAsync(bl => bl.SerialNumber == createGenericUserRequest.SerialNumber);
                if (isOnBlackList)
                {
                    createGoogleUserResponse.IsSuccess = false;
                    createGoogleUserResponse.Message = "You are in Black List 💀";
                }
                else if (!MailUtils.IsEmailValid(createGenericUserRequest.Email!))
                {
                    createGoogleUserResponse.IsSuccess = false;
                    createGoogleUserResponse.Message = "Invalid email format";
                }
                else
                {
                    string friendCode = string.Empty;
                    do
                    {
                        friendCode = GenericUtils.CreateFriendCode(8);
                    }
                    while (await _context.Users.AnyAsync(u => u.FriendCode == friendCode));

                    if (createGenericUserRequest.SerialNumber == "UNKNOWN_IOS" ||
                       createGenericUserRequest.SerialNumber == "UNKNOWN" ||
                       createGenericUserRequest.SerialNumber.Contains("ERROR_"))
                    {
                        string newSerial = string.Empty;
                        do
                        {
                            newSerial = Guid.NewGuid().ToString();
                        }
                        while (await _context.Users.AnyAsync(u => u.SerialNumber == newSerial));

                        createGenericUserRequest.SerialNumber = newSerial;
                    }

                    User user = new User()
                    {
                        Dni = createGenericUserRequest.Dni!,
                        Username = createGenericUserRequest.Username!,
                        Surname = createGenericUserRequest.Surname!,
                        SerialNumber = createGenericUserRequest.SerialNumber!,
                        FriendCode = friendCode,
                        Password = _passwordUtils.PasswordEncoder(createGenericUserRequest.Password!.ToLower(), isGoogleLogin: true),
                        Email = createGenericUserRequest.Email!.ToLower(),
                        Role = GenericUtils.ChangeEnumToIntOnRole(createGenericUserRequest.Role),
                        RoleString = createGenericUserRequest.Role.ToString().ToLower(),
                        InscriptionDate = DateTime.UtcNow
                    };

                    _genericUtils.ClearCache(_userPrefix);

                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    MailUtils.SendEmailAfterCreatedAccountByGoogle(user.Username, user.Email!);

                    createGoogleUserResponse.IsSuccess = true;
                    createGoogleUserResponse.Message = "Usuario creado correctametne";
                }
            }
            catch (Exception ex)
            {
                createGoogleUserResponse.IsSuccess = false;
                createGoogleUserResponse.Message = $"unexpected error on UserRepository -> CreateGoogleUser: {ex.Message}";
            }

            return createGoogleUserResponse;
        }

        public async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest deleteUserRequest)
        {
            DeleteUserResponse deleteUserResponse = new DeleteUserResponse();

            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == deleteUserRequest.Email);
                if (user == null)
                {
                    deleteUserResponse.IsSuccess = false;
                    deleteUserResponse.Message = "User not found with the provided email";
                }
                else
                {
                    List<Step> steps = await _context.Stats.Where(s => s.UserId == user.UserId).ToListAsync();
                    _context.Stats.RemoveRange(steps);

                    List<UserFriend> userFriends = await _context.UserFriends
                        .Where(uf => uf.UserId == user.UserId || uf.FriendId == user.UserId)
                        .ToListAsync();
                    _context.UserFriends.RemoveRange(userFriends);

                    List<Routine> routines = await _context.Routines.Where(r => r.UserId == user.UserId).ToListAsync();

                    foreach (Routine routine in routines)
                    {
                        List<ExerciseProgress> progress = await _context.ExerciseProgress
                            .Where(ep => ep.RoutineId == routine.RoutineId)
                            .ToListAsync();
                        _context.ExerciseProgress.RemoveRange(progress);

                        List<Exercise> exercises = await _context.Exercises
                            .Where(e => e.RoutineId == routine.RoutineId)
                            .ToListAsync();
                        _context.Exercises.RemoveRange(exercises);

                        List<SplitDay> splitDays = await _context.SplitDays
                            .Where(sd => sd.RoutineId == routine.RoutineId)
                            .ToListAsync();
                        _context.SplitDays.RemoveRange(splitDays);
                    }

                    _genericUtils.ClearCache(_userPrefix);
                    _genericUtils.ClearCache(_stepPrefix);
                    _genericUtils.ClearCache(_routinePrefix);
                    _genericUtils.ClearCache(_friendPrefix);
                    _genericUtils.ClearCache(_authPrefix);
                    _genericUtils.ClearCache(_exercisePrefix);

                    _context.Routines.RemoveRange(routines);
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();

                    deleteUserResponse.IsSuccess = true;
                    deleteUserResponse.Message = "User deleted successfully";
                }
            }
            catch (Exception ex)
            {
                deleteUserResponse.IsSuccess = false;
                deleteUserResponse.Message = $"Unexpected error on UserRepository -> DeleteUser: {ex.Message}";
            }

            return deleteUserResponse;
        }

        public async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest updateUserRequest)
        {
            UpdateUserResponse updateUserResponse = new UpdateUserResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == updateUserRequest.OldEmail);
                if (user == null)
                {
                    updateUserResponse.IsSuccess = false;
                    updateUserResponse.Message = "User not found with the provided email";
                }
                else
                {
                    if (!GenericUtils.IsDniValid(updateUserRequest.NewDni!))
                    {
                        updateUserResponse.IsSuccess = false;
                        updateUserResponse.Message = "The dni is not valid you need eight numbers and a capital letter";
                    }
                    else
                    {
                        user.Dni = updateUserRequest.NewDni ?? user.Dni;
                        user.Username = updateUserRequest.NewUsername ?? user.Username;
                        user.Surname = updateUserRequest.NewSurname ?? user.Surname;
                        user.Email = updateUserRequest.NewEmail ?? user.Email;

                        _genericUtils.ClearCache(_userPrefix);
                        await _context.SaveChangesAsync();

                        string newToken = string.Empty;
                        if (user.Email != updateUserRequest.OldEmail)
                        {
                            newToken = user.RoleString.ToLower() == Role.ADMIN.ToString().ToLower()
                                ? JwtUtils.GenerateAdminJwtToken(user.Email)
                                : JwtUtils.GenerateUserJwtToken(user.Email);
                        }

                        updateUserResponse.NewToken = newToken;
                        updateUserResponse.IsSuccess = true;
                        updateUserResponse.UserDTO = UserMapper.UserToDto(user);
                        updateUserResponse.Message = "User updated successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                updateUserResponse.IsSuccess = false;
                updateUserResponse.Message = $"unexpected error on UserRepository -> UpdateUser: {ex.Message}";
            }

            return updateUserResponse;
        }

        public async Task<CreateNewPasswordResponse> CreateNewPassword(CreateNewPasswordRequest createNewPasswordRequest)
        {
            CreateNewPasswordResponse createNewPasswordResponse = new CreateNewPasswordResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == createNewPasswordRequest.UserEmail);
                if (user == null)
                {
                    createNewPasswordResponse.IsSuccess = false;
                    createNewPasswordResponse.Message = "User not found with the provided email";
                }
                else
                {
                    string newPassword = PasswordUtils.GenerateSecurePassword();
                    while (_passwordUtils.VerifyPassword(user.Password!, newPassword))
                    {
                        newPassword = PasswordUtils.GenerateSecurePassword();
                    }
                    user.Password = _passwordUtils.PasswordEncoder(newPassword);

                    _genericUtils.ClearCache(_userPrefix);
                    await _context.SaveChangesAsync();

                    MailUtils.SendEmail(user.Username, user.Email, newPassword);

                    createNewPasswordResponse.IsSuccess = true;
                    createNewPasswordResponse.Message = "New password created successfully";
                }
            }
            catch (Exception ex)
            {
                createNewPasswordResponse.IsSuccess = false;
                createNewPasswordResponse.Message = $"unexpected error on UserRepository -> CreateNewPassword: {ex.Message}";
            }

            return createNewPasswordResponse;
        }

        public async Task<ChangePasswordWithPasswordAndEmailResponse> ChangePasswordWithPasswordAndEmail(ChangePasswordWithPasswordAndEmailRequest changePasswordWithPasswordAndEmailRequest)
        {
            ChangePasswordWithPasswordAndEmailResponse changePasswordWithPasswordAndEmailResponse = new ChangePasswordWithPasswordAndEmailResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == changePasswordWithPasswordAndEmailRequest.UserEmail);
                if (user == null)
                {
                    changePasswordWithPasswordAndEmailResponse.IsSuccess = false;
                    changePasswordWithPasswordAndEmailResponse.Message = "User not found with the provided email";
                }
                else
                {
                    if (!_passwordUtils.VerifyPassword(user.Password!, changePasswordWithPasswordAndEmailRequest.OldPassword!))
                    {
                        changePasswordWithPasswordAndEmailResponse.IsSuccess = false;
                        changePasswordWithPasswordAndEmailResponse.Message = "Old password does not match";
                    }
                    else
                    {
                        if (!PasswordUtils.IsPasswordValid(changePasswordWithPasswordAndEmailRequest.NewPassword!))
                        {
                            changePasswordWithPasswordAndEmailResponse.IsSuccess = false;
                            changePasswordWithPasswordAndEmailResponse.Message = "New password does not meet the required criteria";
                        }
                        else
                        {
                            user.Password = _passwordUtils.PasswordEncoder(changePasswordWithPasswordAndEmailRequest.NewPassword!);

                            _genericUtils.ClearCache(_userPrefix);
                            await _context.SaveChangesAsync();

                            MailUtils.SendEmail(user.Username, user.Email, changePasswordWithPasswordAndEmailRequest.NewPassword!);

                            changePasswordWithPasswordAndEmailResponse.IsSuccess = true;
                            changePasswordWithPasswordAndEmailResponse.Message = "User password changed successfully";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                changePasswordWithPasswordAndEmailResponse.IsSuccess = false;
                changePasswordWithPasswordAndEmailResponse.Message = $"unexpected error on UserRepository -> ChangePasswordWithPasswordAndEmail: {ex.Message}";
            }

            return changePasswordWithPasswordAndEmailResponse;
        }

        public async Task<GetUserProfileDetailsResponse> GetUserProfileDetails(GetUserProfileDetailsRequest GetUserProfileDetails)
        {
            GetUserProfileDetailsResponse getUserProfileDetailsResponse = new GetUserProfileDetailsResponse();
            try
            {
                string cacheKey = $"{_userPrefix}GetUserProfileDetails_{GetUserProfileDetails.UserEmail}";

                User? cachedUser = _cacheUtils.Get<User>(cacheKey);
                if (cachedUser != null)
                {
                    getUserProfileDetailsResponse.IsSuccess = true;
                    getUserProfileDetailsResponse.Message = "User profile details retrieved successfully";
                    getUserProfileDetailsResponse.Username = cachedUser.Username;
                    getUserProfileDetailsResponse.InscriptionDate = cachedUser.InscriptionDate;
                    getUserProfileDetailsResponse.RoutineCount = cachedUser.Routines.Count;
                }
                else
                {
                    User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == GetUserProfileDetails.UserEmail);
                    if (user == null)
                    {
                        getUserProfileDetailsResponse.IsSuccess = false;
                        getUserProfileDetailsResponse.Message = "User not found with the provided email";
                    }
                    else
                    {
                        getUserProfileDetailsResponse.IsSuccess = true;
                        getUserProfileDetailsResponse.Message = "User profile details retrieved successfully";
                        getUserProfileDetailsResponse.Username = user.Username;
                        getUserProfileDetailsResponse.InscriptionDate = user.InscriptionDate;
                        getUserProfileDetailsResponse.RoutineCount = user.Routines.Count;

                        _cacheUtils.Set(cacheKey, user, TimeSpan.FromMinutes(_expiryMinutes));
                    }
                }
            }
            catch (Exception ex)
            {
                getUserProfileDetailsResponse.IsSuccess = false;
                getUserProfileDetailsResponse.Message = $"unexpected error on UserRepository -> GetUserProfileDetails: {ex.Message}";
            }

            return getUserProfileDetailsResponse;
        }

        public async Task<CheckUserExistenceResponse> CheckUserExistence(CheckUserExistenceRequest checkUserExistenceRequest)
        {
            CheckUserExistenceResponse checkUserExistenceResponse = new CheckUserExistenceResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == checkUserExistenceRequest.Email);
                if (user == null)
                {
                    checkUserExistenceResponse.IsSuccess = true;
                    checkUserExistenceResponse.UserExists = false;
                    checkUserExistenceResponse.Message = "User not found";
                }
                else
                {
                    checkUserExistenceResponse.IsSuccess = true;
                    checkUserExistenceResponse.UserExists = true;
                    checkUserExistenceResponse.Message = "User found";
                }
            }
            catch (Exception ex)
            {
                checkUserExistenceResponse.IsSuccess = false;
                checkUserExistenceResponse.UserExists = false;
                checkUserExistenceResponse.Message = $"unexpected error on UserRepository -> CheckUserExistence: {ex.Message}";
            }

            return checkUserExistenceResponse;
        }

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
                    bool isOnBlackList = await _context.BlackList.AnyAsync(bl => bl.SerialNumber == addUserToBlackListRequest.SerialNumber &&
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

        public async Task<GetIntegralUserInfoResponse> GetIntegralUserInfo(GetIntegralUserInfoRequest getIntegralUserInfoRequest)
        {
            GetIntegralUserInfoResponse getIntegralUserInfoResponse = new GetIntegralUserInfoResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == getIntegralUserInfoRequest.UserId);
                if (user == null)
                {
                    getIntegralUserInfoResponse.IsSuccess = false;
                    getIntegralUserInfoResponse.UserDTO = new UserDTO();
                    getIntegralUserInfoResponse.Message = $"User not found";
                }
                else
                {
                    UserDTO userDTO = UserMapper.UserToDto(user);
                    userDTO.Password = PasswordUtils.DecryptPasswordWithMasterKey(user.Password!, getIntegralUserInfoRequest.MasterKey);

                    getIntegralUserInfoResponse.IsSuccess = true;
                    getIntegralUserInfoResponse.UserDTO = userDTO;
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
                        user.Password = PasswordUtils.DecryptPasswordWithMasterKey(u.Password!, getIntegralUsersRequest.MasterKey);
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
    }
}