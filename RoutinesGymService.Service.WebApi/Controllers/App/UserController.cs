using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.GetAllUserFriends;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Check.CheckUserExistence;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.ChangePasswordWithPasswordAndEmail;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateGenericUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateGoogleUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateNewPassword;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.DeleteUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUserByEmail;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUserProfileDetails;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.UpdateUser;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Domain.Model.Enums;
using RoutinesGymService.Transversal.Common.Responses;
using RoutinesGymService.Transversal.JsonInterchange.User.Check.CheckUserExistence;
using RoutinesGymService.Transversal.JsonInterchange.User.Create.ChangePasswordWithPasswordAndEmail;
using RoutinesGymService.Transversal.JsonInterchange.User.Create.CreateGoogleUser;
using RoutinesGymService.Transversal.JsonInterchange.User.Create.CreateNewPassword;
using RoutinesGymService.Transversal.JsonInterchange.User.Create.CreateUser;
using RoutinesGymService.Transversal.JsonInterchange.User.DeleteUser;
using RoutinesGymService.Transversal.JsonInterchange.User.Get.GetUserByEmail;
using RoutinesGymService.Transversal.JsonInterchange.User.Get.GetUserProfileDetails;
using RoutinesGymService.Transversal.JsonInterchange.User.UpdateUser;
using RoutinesGymService.Transversal.Security.SecurityFilters;
using System.Security.Claims;

namespace RoutinesGymService.Service.WebApi.Controllers.App
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserApplication _userApplication;
        private readonly IFriendApplication _friendApplication;

        public UserController(IUserApplication userApplication, IFriendApplication friendApplication)
        {
            _userApplication = userApplication;
            _friendApplication = friendApplication;
        }

        #region Get user by email
        [HttpPost("get-user-by-email")]
        [Authorize]
        [JwtValidationFilter]           
        [ResourceAuthorizationFilter]
        public async Task<ActionResult<GetUserByEmailResponseJson>> GetUserByEmail([FromBody] GetUserByEmailRequestJson getUserByEmailRequestJson)
        {
            GetUserByEmailResponseJson getUserByEmailResponseJson = new GetUserByEmailResponseJson();
            try
            {
                if (string.IsNullOrEmpty(getUserByEmailRequestJson.UserEmail))
                {
                    getUserByEmailResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    getUserByEmailResponseJson.IsSuccess = false;
                    getUserByEmailResponseJson.Message = "invalid data, the email is null or empty";
                }
                else
                {
                    GetUserByEmailRequest getUserByEmailRequest = new GetUserByEmailRequest
                    {
                        UserEmail = getUserByEmailRequestJson.UserEmail,
                    };

                    GetUserByEmailResponse getUserByEmailResponse = await _userApplication.GetUserByEmail(getUserByEmailRequest);
                    if (getUserByEmailResponse.IsSuccess)
                    {
                        getUserByEmailResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        getUserByEmailResponseJson.UserDto = getUserByEmailResponse.UserDto;
                        getUserByEmailResponseJson.FriendsCount = getUserByEmailResponse.FriendsCount;
                        getUserByEmailResponseJson.RoutinesCount = getUserByEmailResponse.RoutinesCount;
                        getUserByEmailResponseJson.IsSuccess = getUserByEmailResponse.IsSuccess;
                        getUserByEmailResponseJson.Message = getUserByEmailResponse.Message;
                    }
                    else
                    {
                        getUserByEmailResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        getUserByEmailResponseJson.UserDto = getUserByEmailResponse.UserDto;
                        getUserByEmailResponseJson.FriendsCount = getUserByEmailResponse.FriendsCount;
                        getUserByEmailResponseJson.RoutinesCount = getUserByEmailResponse.RoutinesCount;
                        getUserByEmailResponseJson.IsSuccess = getUserByEmailResponse.IsSuccess;
                        getUserByEmailResponseJson.Message = getUserByEmailResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                getUserByEmailResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                getUserByEmailResponseJson.IsSuccess = false;
                getUserByEmailResponseJson.Message = $"unexpected error -> UserController -> get-user-by-email: {ex.Message}";
            }

            return Ok(getUserByEmailResponseJson);
        }
        #endregion

        #region Check user existence
        [HttpPost("check-user-existence")]
        public async Task<ActionResult<CheckUserExistenceResponseJson>> CheckUserExistence([FromBody] CheckUserExistenceRequestJson checkUserExistenceRequestJson)
        {
            CheckUserExistenceResponseJson checkUserExistenceResponseJson = new CheckUserExistenceResponseJson();
            try
            {
                if (string.IsNullOrEmpty(checkUserExistenceRequestJson.UserEmail))
                {
                    checkUserExistenceResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    checkUserExistenceResponseJson.Message = "Email is required.";
                    checkUserExistenceResponseJson.IsSuccess = false;
                }
                else
                {
                    CheckUserExistenceRequest checkUserExistenceRequest = new CheckUserExistenceRequest
                    {
                        UserEmail = checkUserExistenceRequestJson.UserEmail,
                    };

                    CheckUserExistenceResponse checkUserExistenceResponse = await _userApplication.CheckUserExistence(checkUserExistenceRequest);
                    if (checkUserExistenceResponse.IsSuccess)
                    {
                        checkUserExistenceResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        checkUserExistenceResponseJson.IsSuccess = checkUserExistenceResponse.IsSuccess;
                        checkUserExistenceResponseJson.UserExists = checkUserExistenceResponse.UserExists;
                        checkUserExistenceResponseJson.Message = checkUserExistenceResponse.Message;
                    }
                    else
                    {
                        checkUserExistenceResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        checkUserExistenceResponseJson.IsSuccess = checkUserExistenceResponse.IsSuccess;
                        checkUserExistenceResponseJson.UserExists = checkUserExistenceResponse.UserExists;
                        checkUserExistenceResponseJson.Message = checkUserExistenceResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                checkUserExistenceResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                checkUserExistenceResponseJson.IsSuccess = false;
                checkUserExistenceResponseJson.Message = $"Unexpected error: {ex.Message}";
            }

            return Ok(checkUserExistenceResponseJson);
        }
        #endregion

        #region Create user
        [HttpPost("create-user")]
        public async Task<ActionResult<CreateUserResponseJson>> CreateUser([FromBody] CreateUserRequestJson createUserRequestJson)
        {
            CreateUserResponseJson createUserResponseJson = new CreateUserResponseJson();
            try
            {
                if (string.IsNullOrEmpty(createUserRequestJson.Dni) ||
                    string.IsNullOrEmpty(createUserRequestJson.Username) ||
                    string.IsNullOrEmpty(createUserRequestJson.SerialNumber) ||
                    string.IsNullOrEmpty(createUserRequestJson.UserEmail) ||
                    string.IsNullOrEmpty(createUserRequestJson.Password) ||
                    string.IsNullOrEmpty(createUserRequestJson.ConfirmPassword))
                {
                    createUserResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    createUserResponseJson.IsSuccess = false;
                    createUserResponseJson.Message = "invalid data, the data is null or empty";
                }
                else
                {
                    CreateGenericUserRequest createGenericUserRequest = new CreateGenericUserRequest
                    {
                        Dni = createUserRequestJson.Dni.Trim(),
                        Username = createUserRequestJson.Username.Trim(),
                        UserEmail = createUserRequestJson.UserEmail.Trim(),
                        Password = createUserRequestJson.Password.Trim(),
                        ConfirmPassword = createUserRequestJson.ConfirmPassword.Trim(),
                        SerialNumber = createUserRequestJson.SerialNumber,
                        Role = Role.USER
                    };

                    CreateUserResponse createUserResponse = await _userApplication.CreateUser(createGenericUserRequest);
                    if (createUserResponse.IsSuccess)
                    {
                        createUserResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        createUserResponseJson.IsSuccess = createUserResponse.IsSuccess;
                        createUserResponseJson.Message = createUserResponse.Message;
                    }
                    else
                    {
                        createUserResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        createUserResponseJson.IsSuccess = createUserResponse.IsSuccess;
                        createUserResponseJson.Message = createUserResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                createUserResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                createUserResponseJson.IsSuccess = false;
                createUserResponseJson.Message = $"unexpected error on UserController -> create-user {ex.Message}";
            }

            return Ok(createUserResponseJson);
        }
        #endregion

        #region Create google user
        [HttpPost("create-google-user")]
        public async Task<ActionResult<CreateGoogleUserResponseJson>> CreateGoogleUser([FromBody] CreateGoogleUserRequestJson createGoogleUserRequestJson)
        {
            CreateGoogleUserResponseJson createGoogleUserResponseJson = new CreateGoogleUserResponseJson();
            try
            {
                if (string.IsNullOrEmpty(createGoogleUserRequestJson.Username) ||
                    string.IsNullOrEmpty(createGoogleUserRequestJson.Email) ||
                    string.IsNullOrEmpty(createGoogleUserRequestJson.Password) ||
                    string.IsNullOrEmpty(createGoogleUserRequestJson.SerialNumber) ||
                    string.IsNullOrEmpty(createGoogleUserRequestJson.ConfirmPassword))
                {
                    createGoogleUserResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    createGoogleUserResponseJson.IsSuccess = false;
                    createGoogleUserResponseJson.Message = "invalid data, the data is null or empty";
                }
                else
                {
                    CreateGenericUserRequest createGenericUserRequest = new CreateGenericUserRequest
                    {
                        Dni = createGoogleUserRequestJson.Dni,
                        Username = createGoogleUserRequestJson.Username,
                        Surname = createGoogleUserRequestJson.Surname,
                        UserEmail = createGoogleUserRequestJson.Email,
                        Password = createGoogleUserRequestJson.Email, // no se puede sacar la contraseña desde google, a si que se usa el email como contraseña
                        SerialNumber = createGoogleUserRequestJson.SerialNumber,
                        Role = Role.USER
                    };

                    CreateGoogleUserResponse createGoogleUserResponse = await _userApplication.CreateGoogleUser(createGenericUserRequest);
                    if (createGoogleUserResponse.IsSuccess)
                    {
                        createGoogleUserResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        createGoogleUserResponseJson.IsSuccess = createGoogleUserResponse.IsSuccess;
                        createGoogleUserResponseJson.Message = createGoogleUserResponse.Message;
                    }
                    else
                    {
                        createGoogleUserResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        createGoogleUserResponseJson.IsSuccess = createGoogleUserResponse.IsSuccess;
                        createGoogleUserResponseJson.Message = createGoogleUserResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                createGoogleUserResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                createGoogleUserResponseJson.IsSuccess = false;
                createGoogleUserResponseJson.Message = $"unexpected error on UserController -> create-google-user: {ex.Message}";
            }

            return Ok(createGoogleUserResponseJson);
        }
        #endregion

        #region Update user
        [HttpPost("update-user")]
        [Authorize]
        [JwtValidationFilter]
        [ResourceAuthorizationFilter]
        public async Task<ActionResult<UpdateUserResponseJson>> UpdateUser([FromBody] UpdateUserRequestJson updateUserRequestJson)
        {
            UpdateUserResponseJson updateUserResponseJson = new UpdateUserResponseJson();
            try
            {
                if (string.IsNullOrEmpty(updateUserRequestJson.OriginalUserEmail))
                {
                    updateUserResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    updateUserResponseJson.IsSuccess = false;
                    updateUserResponseJson.Message = "invalid data, the email is null or empty";
                }
                else
                {
                    UpdateUserRequest updateUserRequest = new UpdateUserRequest
                    {
                        OriginalUserEmail = updateUserRequestJson.OriginalUserEmail,
                        NewDni = updateUserRequestJson.DniToBeFound,
                        NewUsername = updateUserRequestJson.Username,
                        NewSurname = updateUserRequestJson.Surname,
                        NewEmail = updateUserRequestJson.Email,
                    };

                    UpdateUserResponse updateUserResponse = await _userApplication.UpdateUser(updateUserRequest);
                    if (updateUserResponse.IsSuccess)
                    {
                        updateUserResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        updateUserResponseJson.UserDto = updateUserResponse.UserDto;
                        updateUserResponseJson.NewToken = updateUserResponse.NewToken;
                        updateUserResponseJson.IsSuccess = updateUserResponse.IsSuccess;
                        updateUserResponseJson.Message = updateUserResponse.Message;
                    }
                    else
                    {
                        updateUserResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        updateUserResponseJson.UserDto = updateUserResponse.UserDto;
                        updateUserResponseJson.NewToken = updateUserResponse.NewToken;
                        updateUserResponseJson.IsSuccess = updateUserResponse.IsSuccess;
                        updateUserResponseJson.Message = updateUserResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                updateUserResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                updateUserResponseJson.IsSuccess = false;
                updateUserResponseJson.Message = $"unexpected error on UserController -> update-user: {ex.Message}";
            }

            return Ok(updateUserResponseJson);
        }
        #endregion

        #region Delete user
        [HttpPost("delete-user")]
        [Authorize]
        [JwtValidationFilter]
        [ResourceAuthorizationFilter]
        public async Task<ActionResult<DeleteUserResponseJson>> DeleteUser([FromBody] DeleteUserRequestJson deleteUserRequestJson)
        {
            DeleteUserResponseJson deleteUserResponseJson = new DeleteUserResponseJson();
            try
            {
                if (string.IsNullOrEmpty(deleteUserRequestJson.UserEmail))
                {
                    deleteUserResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    deleteUserResponseJson.IsSuccess = false;
                    deleteUserResponseJson.Message = "Invalid data, the email is null or empty";
                }
                else
                {
                    DeleteUserRequest deleteUserRequest = new DeleteUserRequest
                    {
                        UserEmail = deleteUserRequestJson.UserEmail
                    };

                    DeleteUserResponse deleteUserResponse = await _userApplication.DeleteUser(deleteUserRequest);
                    if (deleteUserResponse.IsSuccess)
                    {
                        deleteUserResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        deleteUserResponseJson.IsSuccess = deleteUserResponse.IsSuccess;
                        deleteUserResponseJson.Message = deleteUserResponse.Message;

                    }
                    else
                    {
                        deleteUserResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        deleteUserResponseJson.IsSuccess = deleteUserResponse.IsSuccess;
                        deleteUserResponseJson.Message = deleteUserResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                deleteUserResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                deleteUserResponseJson.IsSuccess = false;
                deleteUserResponseJson.Message = $"unexpected error on -> UserController -> delete-user: {ex.Message}";
            }

            return Ok(deleteUserResponseJson);
        }
        #endregion

        #region Create new password
        [HttpPost("create-new-password")]
        [Authorize]
        [JwtValidationFilter]
        [ResourceAuthorizationFilter]
        public async Task<ActionResult<CreateNewPasswordResponseJson>> CreateNewPassword([FromBody] CreateNewPasswordRequestJson createNewPasswordRequestJson)
        {
            CreateNewPasswordResponseJson createNewPasswordResponseJson = new CreateNewPasswordResponseJson();
            try
            {
                if (string.IsNullOrEmpty(createNewPasswordRequestJson.UserEmail))
                {
                    createNewPasswordResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    createNewPasswordResponseJson.IsSuccess = false;
                    createNewPasswordResponseJson.Message = "Invalid data, the email is null or empty";
                }
                else
                {
                    CreateNewPasswordRequest createNewPasswordRequest = new CreateNewPasswordRequest
                    {
                        UserEmail = createNewPasswordRequestJson.UserEmail,
                    };

                    CreateNewPasswordResponse createNewPasswordResponse = await _userApplication.CreateNewPassword(createNewPasswordRequest);
                    if (createNewPasswordResponse.IsSuccess)
                    {
                        createNewPasswordResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        createNewPasswordResponseJson.IsSuccess = createNewPasswordResponse.IsSuccess;
                        createNewPasswordResponseJson.Message = createNewPasswordResponse.Message;
                    }
                    else
                    {
                        createNewPasswordResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        createNewPasswordResponseJson.IsSuccess = createNewPasswordResponse.IsSuccess;
                        createNewPasswordResponseJson.Message = createNewPasswordResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                createNewPasswordResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                createNewPasswordResponseJson.IsSuccess = false;
                createNewPasswordResponseJson.Message = $"unexpected error on UserController -> create-new-password: {ex.Message}";
            }

            return Ok(createNewPasswordResponseJson);
        }
        #endregion

        #region Change password with password and email
        [HttpPost("change-password-with-password-and-email")]
        [Authorize]
        [JwtValidationFilter]
        [ResourceAuthorizationFilter]
        public async Task<ActionResult<ChangePasswordWithPasswordAndEmailResponseJson>> ChangePasswordWithPasswordAndEmail([FromBody] ChangePasswordWithPasswordAndEmailRequestJson changePasswordWithPasswordAndEmailRequestJson)
        {
            ChangePasswordWithPasswordAndEmailResponseJson changePasswordWithPasswordAndEmailResponseJson = new ChangePasswordWithPasswordAndEmailResponseJson();
            try
            {
                if (string.IsNullOrEmpty(changePasswordWithPasswordAndEmailRequestJson.UserEmail) ||
                    string.IsNullOrEmpty(changePasswordWithPasswordAndEmailRequestJson.OldPassword) ||
                    string.IsNullOrEmpty(changePasswordWithPasswordAndEmailRequestJson.NewPassword) ||
                    string.IsNullOrEmpty(changePasswordWithPasswordAndEmailRequestJson.ConfirmNewPassword))
                {
                    changePasswordWithPasswordAndEmailResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    changePasswordWithPasswordAndEmailResponseJson.IsSuccess = false;
                    changePasswordWithPasswordAndEmailResponseJson.Message = "invalid data the data is null or empty";
                }
                else
                {
                    ChangePasswordWithPasswordAndEmailRequest changePasswordWithPasswordAndEmailRequest = new ChangePasswordWithPasswordAndEmailRequest
                    {
                         UserEmail = changePasswordWithPasswordAndEmailRequestJson.UserEmail,
                         OldPassword = changePasswordWithPasswordAndEmailRequestJson.OldPassword,
                         NewPassword = changePasswordWithPasswordAndEmailRequestJson.NewPassword,
                         ConfirmNewPassword = changePasswordWithPasswordAndEmailRequestJson.ConfirmNewPassword,
                    };

                    ChangePasswordWithPasswordAndEmailResponse changePasswordWithPasswordAndEmailResponse = await _userApplication.ChangePasswordWithPasswordAndEmail(changePasswordWithPasswordAndEmailRequest);
                    if (changePasswordWithPasswordAndEmailResponse.IsSuccess)
                    {
                        changePasswordWithPasswordAndEmailResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        changePasswordWithPasswordAndEmailResponseJson.IsSuccess = changePasswordWithPasswordAndEmailResponse.IsSuccess;
                        changePasswordWithPasswordAndEmailResponseJson.Message = changePasswordWithPasswordAndEmailResponse.Message;
                    }
                    else
                    {
                        changePasswordWithPasswordAndEmailResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        changePasswordWithPasswordAndEmailResponseJson.IsSuccess = changePasswordWithPasswordAndEmailResponse.IsSuccess;
                        changePasswordWithPasswordAndEmailResponseJson.Message = changePasswordWithPasswordAndEmailResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                changePasswordWithPasswordAndEmailResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                changePasswordWithPasswordAndEmailResponseJson.IsSuccess = false;
                changePasswordWithPasswordAndEmailResponseJson.Message = $"unexpected error on UserController -> change-password-with-password-and-email: {ex.Message}";
            }

            return Ok(changePasswordWithPasswordAndEmailResponseJson);
        }
        #endregion

        #region Get user profile details
        [HttpPost("get-user-profile-details")]
        [Authorize]
        public async Task<ActionResult<GetUserProfileDetailsResponseJson>> GetUserProfileDetails([FromBody] GetUserProfileDetailsRequestJson getUserProfileDetailsRequestJson)
        {
            GetUserProfileDetailsResponseJson getUserProfileDetailsResponseJson = new GetUserProfileDetailsResponseJson();
            try
            {
                string? tokenEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(tokenEmail))
                {
                    getUserProfileDetailsResponseJson.ResponseCodeJson = ResponseCodesJson.UNAUTHORIZED;
                    getUserProfileDetailsResponseJson.IsSuccess = false;
                    getUserProfileDetailsResponseJson.Message = "UNAUTHORIZED";
                }
                else if (string.IsNullOrEmpty(getUserProfileDetailsRequestJson.UserEmail))
                {
                    getUserProfileDetailsResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    getUserProfileDetailsResponseJson.IsSuccess = false;
                    getUserProfileDetailsResponseJson.Message = "invalid data, the data is null or empty";
                }
                else
                {
                    string requestedEmail = getUserProfileDetailsRequestJson.UserEmail;
                    bool isOwnProfile = requestedEmail == tokenEmail;

                    GetAllUserFriendsRequest getAllUserFriendsRequest = new GetAllUserFriendsRequest
                    {
                        UserEmail = tokenEmail
                    };

                    GetAllUserFriendsResponse getAllUserFriendsResponse = await _friendApplication.GetAllUserFriends(getAllUserFriendsRequest);
                    bool areFriends = getAllUserFriendsResponse.Friends.Any(f => f.Email == requestedEmail);
                    bool isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "ADMIN";

                    if (!isOwnProfile && !areFriends && !isAdmin)
                    {
                        getUserProfileDetailsResponseJson.ResponseCodeJson = ResponseCodesJson.UNAUTHORIZED;
                        getUserProfileDetailsResponseJson.IsSuccess = false;
                        getUserProfileDetailsResponseJson.Message = "UNAUTHORIZED";
                    }
                    else
                    {
                        GetUserProfileDetailsRequest getUserProfileDetailsRequest = new GetUserProfileDetailsRequest
                        {
                            UserEmail = requestedEmail,
                        };

                        GetUserProfileDetailsResponse getUserProfileDetailsResponse = await _userApplication.GetUserProfileDetails(getUserProfileDetailsRequest);
                        if (getUserProfileDetailsResponse.IsSuccess)
                        {
                            getUserProfileDetailsResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                            getUserProfileDetailsResponseJson.Username = getUserProfileDetailsResponse.Username;
                            getUserProfileDetailsResponseJson.InscriptionDate = getUserProfileDetailsResponse.InscriptionDate;
                            getUserProfileDetailsResponseJson.RoutineCount = getUserProfileDetailsResponse.RoutineCount;
                            getUserProfileDetailsResponseJson.IsSuccess = getUserProfileDetailsResponse.IsSuccess;
                            getUserProfileDetailsResponseJson.Message = getUserProfileDetailsResponse.Message;
                        }
                        else
                        {
                            getUserProfileDetailsResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                            getUserProfileDetailsResponseJson.Username = getUserProfileDetailsResponse.Username;
                            getUserProfileDetailsResponseJson.InscriptionDate = getUserProfileDetailsResponse.InscriptionDate;
                            getUserProfileDetailsResponseJson.RoutineCount = getUserProfileDetailsResponse.RoutineCount;
                            getUserProfileDetailsResponseJson.IsSuccess = getUserProfileDetailsResponse.IsSuccess;
                            getUserProfileDetailsResponseJson.Message = getUserProfileDetailsResponse.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                getUserProfileDetailsResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                getUserProfileDetailsResponseJson.IsSuccess = false;
                getUserProfileDetailsResponseJson.Message = $"unexpected error on UserController -> get-user-profile-details {ex.Message}";
            }

            return Ok(getUserProfileDetailsResponseJson);
        }
        #endregion
    }
}