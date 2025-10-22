using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RoutinesGymApp.Domain.Entities;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Check.CheckUserExistence;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Check.UserExist;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.ChangePasswordWithPasswordAndEmail;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateGenericUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateGoogleUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateNewPassword;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.DeleteUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUserByEmail;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUserProfileDetails;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.UpdateUser;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Application.Mapper;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Domain.Model.Enums;
using RoutinesGymService.Infraestructure.Persistence.Context;
using RoutinesGymService.Transversal.Common.Responses;
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
            _expiryMinutes = int.TryParse(configuration["CacheSettings:CacheExpiryMinutes"], out var m) ? m : 60;
        }

        #region Get users by email
        public async Task<GetUserByEmailResponse> GetUserByEmail(GetUserByEmailRequest getUserByEmailRequest)
        {
            GetUserByEmailResponse getUserByEmailResponse = new GetUserByEmailResponse();
            try
            {
                string cacheKey = $"{_userPrefix}GetUserByEmail_{getUserByEmailRequest.UserEmail}";

                User? cachedUser = _cacheUtils.Get<User>(cacheKey);
                if (cachedUser != null)
                {
                    getUserByEmailResponse.IsSuccess = true;
                    getUserByEmailResponse.Message = "User found successfully";
                    getUserByEmailResponse.RoutinesCount = cachedUser.Routines.Count;
                    getUserByEmailResponse.FriendsCount = await _context.UserFriends.CountAsync(u => u.UserId == cachedUser.UserId);
                    getUserByEmailResponse.UserDto = UserMapper.UserToDto(cachedUser);
                }
                else
                {
                    User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == getUserByEmailRequest.UserEmail);
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
                            getUserByEmailResponse.UserDto = UserMapper.UserToDto(user);

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
        #endregion

        #region Create user
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
                else if (!GenericUtils.IsDniValid(createGenericUserRequest.Dni))
                {
                    createUserResponse.IsSuccess = false;
                    createUserResponse.Message = "The dni is not valid you need eight numbers and a capital letter";
                }
                else if (!MailUtils.IsEmailValid(createGenericUserRequest.UserEmail))
                {
                    createUserResponse.IsSuccess = false;
                    createUserResponse.Message = "Invalid email format example (example@gmail.com)";
                }
                else if (!PasswordUtils.IsPasswordValid(createGenericUserRequest.Password))
                {
                    createUserResponse.IsSuccess = false;
                    createUserResponse.Message = "Password does not meet the required criteria you need: eight characters with one upper case, one lower case, one number and one special character.";
                }
                else if (createGenericUserRequest.ConfirmPassword != createGenericUserRequest.Password)
                {
                    createUserResponse.IsSuccess = false;
                    createUserResponse.Message = "Password and confirm password do not match";
                }
                else
                {
                    UserExistRequest userExistRequest = new UserExistRequest
                    {
                        Dni = createGenericUserRequest.Dni,
                        UserEmail = createGenericUserRequest.UserEmail
                    };

                    /* Si existe un usuario con el mismo email o con el mismo dni o con los dos error */
                    UserExistResponse userExistResponse = await UserExist(userExistRequest);
                    if (userExistResponse.UserExist)
                    {
                        createUserResponse.IsSuccess = false;
                        createUserResponse.Message = userExistResponse.Message;
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
                            Dni = createGenericUserRequest.Dni,
                            Username = createGenericUserRequest.Username,
                            Surname = createGenericUserRequest.Surname ?? "",
                            Email = createGenericUserRequest.UserEmail.ToLower(),
                            FriendCode = friendCode,
                            SerialNumber = createGenericUserRequest.SerialNumber,
                            Password = _passwordUtils.PasswordEncoder(createGenericUserRequest.Password),
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
            catch (Exception ex)
            {
                createUserResponse.Message = $"unexpected error on UserRepository -> CreateUser: {ex.Message}";
                createUserResponse.IsSuccess = false;
            }

            return createUserResponse;
        }
        #endregion

        #region Create google user
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
                else if (!MailUtils.IsEmailValid(createGenericUserRequest.UserEmail))
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
                        Dni = createGenericUserRequest.Dni,
                        Username = createGenericUserRequest.Username,
                        Surname = createGenericUserRequest.Surname,
                        SerialNumber = createGenericUserRequest.SerialNumber,
                        FriendCode = friendCode,
                        Password = _passwordUtils.PasswordEncoder(createGenericUserRequest.Password.ToLower(), isGoogleLogin: true),
                        Email = createGenericUserRequest.UserEmail.ToLower(),
                        Role = GenericUtils.ChangeEnumToIntOnRole(createGenericUserRequest.Role),
                        RoleString = createGenericUserRequest.Role.ToString().ToLower(),
                        InscriptionDate = DateTime.UtcNow
                    };

                    _genericUtils.ClearCache(_userPrefix);

                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    MailUtils.SendEmailAfterCreatedAccountByGoogle(user.Username, user.Email);

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
        #endregion

        #region Delete user
        public async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest deleteUserRequest)
        {
            DeleteUserResponse deleteUserResponse = new DeleteUserResponse();
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var userData = await _context.Users
                    .Where(u => u.Email == deleteUserRequest.UserEmail)
                    .GroupJoin(_context.Steps,
                        u => u.UserId,
                        s => s.UserId,
                        (u, stats) => new { User = u, Stats = stats })
                    .SelectMany(x => x.Stats.DefaultIfEmpty(),
                        (x, stat) => new { x.User, Stat = stat })
                    .GroupJoin(_context.UserFriends,
                        x => x.User.UserId,
                        uf => uf.UserId,
                        (x, friends) => new { x.User, x.Stat, Friends = friends })
                    .SelectMany(x => x.Friends.DefaultIfEmpty(),
                        (x, friend) => new { x.User, x.Stat, Friend = friend })
                    .GroupJoin(_context.Routines,
                        x => x.User.UserId,
                        r => r.UserId,
                        (x, routines) => new { x.User, x.Stat, x.Friend, Routines = routines })
                    .ToListAsync();

                if (!userData.Any())
                {
                    deleteUserResponse.IsSuccess = false;
                    deleteUserResponse.Message = "User not found with the provided email";
                }
                else
                {
                    User user = userData.First().User;
                    long userId = user.UserId;

                    /* Primero eliminamos los pasos y los amigos */
                    List<Step> steps = await _context.Steps.Where(s => s.UserId == userId).ToListAsync();
                    List<UserFriend> userFriends = await _context.UserFriends.Where(uf => uf.UserId == userId || uf.FriendId == userId).ToListAsync();
                    
                    _context.Steps.RemoveRange(steps);
                    _context.UserFriends.RemoveRange(userFriends);

                    List<long> routineIds = await _context.Routines
                        .Where(r => r.UserId == userId)
                        .Select(r => r.RoutineId)
                        .ToListAsync();

                    if (routineIds.Any())
                    {
                        /* Luego eliminamos el progreso de los ejercicios, los ejerciicos y los splits */
                        List<ExerciseProgress> exerciseProgresses = await _context.ExerciseProgress.Where(ep => routineIds.Contains(ep.RoutineId)).ToListAsync();
                        List<Exercise> exercises = await _context.Exercises.Where(e => routineIds.Contains(e.RoutineId)).ToListAsync();
                        List<SplitDay> splitDays = await _context.SplitDays.Where(sd => routineIds.Contains(sd.RoutineId)).ToListAsync();

                        _context.ExerciseProgress.RemoveRange(exerciseProgresses);
                        _context.Exercises.RemoveRange(exercises);
                        _context.SplitDays.RemoveRange(splitDays);
                    }

                    /* Por ultimo eliminamos las rutinas y el usuario */
                    List<Routine> routines = await _context.Routines.Where(r => r.UserId == userId).ToListAsync();
                    
                    _context.Routines.RemoveRange(routines);
                    _context.Users.Remove(user);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _genericUtils.ClearCache($"{_userPrefix}:{userId}");
                    _genericUtils.ClearCache($"{_stepPrefix}:{userId}");
                    _genericUtils.ClearCache($"{_routinePrefix}:{userId}");
                    _genericUtils.ClearCache($"{_friendPrefix}:{userId}");
                    _genericUtils.ClearCache($"{_authPrefix}:{deleteUserRequest.UserEmail}");

                    deleteUserResponse.IsSuccess = true;
                    deleteUserResponse.Message = "User deleted successfully";
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                deleteUserResponse.IsSuccess = false;
                deleteUserResponse.Message = $"Unexpected error on UserRepository -> DeleteUser: {ex.Message}";
            }

            return deleteUserResponse;
        }
        #endregion

        #region Update user
        public async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest updateUserRequest)
        {
            UpdateUserResponse updateUserResponse = new UpdateUserResponse();
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == updateUserRequest.OriginalUserEmail);
                if (user == null)
                {
                    updateUserResponse.IsSuccess = false;
                    updateUserResponse.Message = "User not found with the provided email";
                }
                else
                {
                    bool hasError = false;
                    if (!string.IsNullOrEmpty(updateUserRequest.NewDni) && updateUserRequest.NewDni != user.Dni)
                    {
                        if (!GenericUtils.IsDniValid(updateUserRequest.NewDni))
                        {
                            updateUserResponse.IsSuccess = false;
                            updateUserResponse.Message = "The DNI is not valid. You need eight numbers and a capital letter";
                            hasError = true;
                        }
                        else
                        {
                            UserExistRequest userExistRequest = new UserExistRequest
                            {
                                Dni = updateUserRequest.NewDni,
                            };

                            UserExistResponse dniExistsResponse = await UserExist(userExistRequest);
                            if (dniExistsResponse.DniExists)
                            {
                                updateUserResponse.IsSuccess = false;
                                updateUserResponse.Message = "The DNI is already registered by another user";
                                hasError = true;
                            }
                        }
                    }

                    if (!hasError && !string.IsNullOrEmpty(updateUserRequest.NewEmail) && updateUserRequest.NewEmail != user.Email)
                    {
                        UserExistRequest userExistRequest = new UserExistRequest
                        {
                            UserEmail = updateUserRequest.NewEmail
                        };

                        UserExistResponse emailExistsResponse = await UserExist(userExistRequest);
                        if (emailExistsResponse.EmailExists)
                        {
                            updateUserResponse.IsSuccess = false;
                            updateUserResponse.Message = "The email is already registered by another user";
                            hasError = true;
                        }
                    }

                    if (!hasError)
                    {
                        bool emailChanged = false;
                        string oldEmail = user.Email;

                        if (!string.IsNullOrEmpty(updateUserRequest.NewDni))
                        {
                            user.Dni = updateUserRequest.NewDni;
                        }

                        if (!string.IsNullOrEmpty(updateUserRequest.NewUsername))
                        {
                            user.Username = updateUserRequest.NewUsername;
                        }

                        if (!string.IsNullOrEmpty(updateUserRequest.NewSurname))
                        {
                            user.Surname = updateUserRequest.NewSurname;
                        }

                        if (!string.IsNullOrEmpty(updateUserRequest.NewEmail) &&
                            updateUserRequest.NewEmail != oldEmail)
                        {
                            user.Email = updateUserRequest.NewEmail;
                            emailChanged = true;
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _genericUtils.ClearCache($"{_userPrefix}:{user.UserId}");
                        _genericUtils.ClearCache($"{_authPrefix}:{oldEmail}");

                        if (emailChanged)
                        {
                            _genericUtils.ClearCache($"{_authPrefix}:{user.Email}");
                        }

                        string newToken = string.Empty;
                        if (emailChanged)
                        {
                            newToken = user.RoleString.Equals(Role.ADMIN.ToString(), StringComparison.OrdinalIgnoreCase)
                                ? JwtUtils.GenerateAdminJwtToken(user.Email)
                                : JwtUtils.GenerateUserJwtToken(user.Email);
                        }

                        updateUserResponse.NewToken = newToken;
                        updateUserResponse.IsSuccess = true;
                        updateUserResponse.UserDto = UserMapper.UserToDto(user);
                        updateUserResponse.Message = "User updated successfully";
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                updateUserResponse.IsSuccess = false;
                updateUserResponse.Message = $"Unexpected error on UserRepository -> UpdateUser: {ex.Message}";
            }

            return updateUserResponse;
        }
        #endregion

        #region Create new password
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
                    while (_passwordUtils.VerifyPassword(user.Password, newPassword))
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
        #endregion

        #region Change password with password and email
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
                    if (!_passwordUtils.VerifyPassword(user.Password, changePasswordWithPasswordAndEmailRequest.OldPassword))
                    {
                        changePasswordWithPasswordAndEmailResponse.IsSuccess = false;
                        changePasswordWithPasswordAndEmailResponse.Message = "Old password does not match";
                    }
                    else
                    {
                        if (!PasswordUtils.IsPasswordValid(changePasswordWithPasswordAndEmailRequest.NewPassword))
                        {
                            changePasswordWithPasswordAndEmailResponse.IsSuccess = false;
                            changePasswordWithPasswordAndEmailResponse.Message = "New password does not meet the required criteria";
                        }
                        else
                        {
                            user.Password = _passwordUtils.PasswordEncoder(changePasswordWithPasswordAndEmailRequest.NewPassword);

                            _genericUtils.ClearCache(_userPrefix);
                            await _context.SaveChangesAsync();

                            MailUtils.SendEmail(user.Username, user.Email, changePasswordWithPasswordAndEmailRequest.NewPassword);

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
        #endregion

        #region Get user profile details
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
        #endregion

        #region Check user existence
        public async Task<CheckUserExistenceResponse> CheckUserExistence(CheckUserExistenceRequest checkUserExistenceRequest)
        {
            CheckUserExistenceResponse checkUserExistenceResponse = new CheckUserExistenceResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == checkUserExistenceRequest.UserEmail);
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
        #endregion

        #region Auxiliary methods

        #region User exist
        /* Este metodo auxiliar se usa en el create user */
        private async Task<UserExistResponse> UserExist(UserExistRequest userExistRequest)
        {
            UserExistResponse userExistResponse = new UserExistResponse();
            try
            {
                bool existsDni = await _context.Users.AnyAsync(u => u.Dni == userExistRequest.Dni);
                bool existsEmail = await _context.Users.AnyAsync(u => u.Email == userExistRequest.UserEmail);

                if (existsDni && existsEmail)
                {
                    userExistResponse.EmailExists = true;
                    userExistResponse.DniExists = true;
                    userExistResponse.Message = "Both DNI and Email exist";
                }
                else if (existsDni)
                {
                    userExistResponse.EmailExists = false;
                    userExistResponse.DniExists = true;
                    userExistResponse.Message = "DNI exists but Email does not exist";
                }
                else if (existsEmail)
                {
                    userExistResponse.EmailExists = true;
                    userExistResponse.DniExists = false;
                    userExistResponse.Message = "Email exists but DNI does not exist";
                }

                userExistResponse.ResponsCode = ResponseCodes.OK;
                userExistResponse.IsSuccess = true;

                userExistResponse.UserExist = userExistResponse != null && userExistResponse.IsSuccess && (existsDni || existsEmail);
            }
            catch (Exception)
            {
                userExistResponse.ResponsCode = ResponseCodes.INTERNAL_SERVER_ERROR;
                userExistResponse.Message = "An error occurred while checking user existence";
                userExistResponse.IsSuccess = false;
                userExistResponse.DniExists = false;
                userExistResponse.EmailExists = false;
                userExistResponse.UserExist = false;
            }

            return userExistResponse;
        }
        #endregion

        #endregion
    }
}