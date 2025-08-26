using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.ChangePasswordWithPasswordAndEmail;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateGenericUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateGoogleUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateNewPassword;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.DeleteUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUserByEmail;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUserProfileDetails;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUsers;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.UpdateUser;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Domain.Model.Enums;
using RoutinesGymService.Transversal.Common;
using RoutinesGymService.Transversal.JsonInterchange.User.Create.ChangePasswordWithPasswordAndEmail;
using RoutinesGymService.Transversal.JsonInterchange.User.Create.CreateAdmin;
using RoutinesGymService.Transversal.JsonInterchange.User.Create.CreateGoogleUser;
using RoutinesGymService.Transversal.JsonInterchange.User.Create.CreateNewPassword;
using RoutinesGymService.Transversal.JsonInterchange.User.Create.CreateUser;
using RoutinesGymService.Transversal.JsonInterchange.User.DeleteUser;
using RoutinesGymService.Transversal.JsonInterchange.User.Get.GetUserByEmail;
using RoutinesGymService.Transversal.JsonInterchange.User.Get.GetUserProfileDetails;
using RoutinesGymService.Transversal.JsonInterchange.User.Get.GetUsers;
using RoutinesGymService.Transversal.JsonInterchange.User.UpdateUser;

namespace RoutinesGymService.Service.WebApi.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserApplication _userApplication;

        public UserController(IUserApplication userApplication)
        {
            _userApplication = userApplication;
        }

        #region Get users
        [HttpGet("get-users")]
        //[Authorize(Roles = nameof(Role.ADMIN))]
        public async Task<ActionResult<GetUsersResponseJson>> GetUsers()
        {
            GetUsersResponseJson getUsersResponseJson = new GetUsersResponseJson();
            getUsersResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            { 
                GetUsersResponse getUsersResponse = await _userApplication.GetUsers();
                if (getUsersResponse.IsSuccess)
                {
                    getUsersResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                    getUsersResponseJson.IsSuccess = getUsersResponse.IsSuccess;
                    getUsersResponseJson.Message = getUsersResponse.Message;
                    getUsersResponseJson.UsersDTO = getUsersResponse.UsersDTO;
                }
                else
                {
                    getUsersResponseJson.IsSuccess = getUsersResponse.IsSuccess;
                    getUsersResponseJson.Message = getUsersResponse.Message;
                }
            }
            catch (Exception ex)
            {
                getUsersResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                getUsersResponseJson.IsSuccess = false;
                getUsersResponseJson.Message = $"unexpected error on UserController -> get-users {ex.Message}";
            }

            return Ok(getUsersResponseJson);
        }
        #endregion

        #region Get user by email
        [HttpPost("get-user-by-email")]
        public async Task<ActionResult<GetUserByEmailResponseJson>> GetUserByEmail([FromBody] GetUserByEmailRequestJson getUserByEmailRequestJson)
        {
            GetUserByEmailResponseJson getUserByEmailResponseJson = new GetUserByEmailResponseJson();
            getUserByEmailResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (getUserByEmailRequestJson == null || 
                    string.IsNullOrEmpty(getUserByEmailRequestJson?.Email))
                {
                    getUserByEmailResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA; 
                    getUserByEmailResponseJson.IsSuccess = false;
                    getUserByEmailResponseJson.Message = "invalid data the user email is null or empty";
                }
                else
                {
                    GetUserByEmailRequest getUserByEmailRequest = new GetUserByEmailRequest
                    {
                        Email = getUserByEmailRequestJson.Email,
                    };

                    GetUserByEmailResponse getUserByEmailResponse = await _userApplication.GetUserByEmail(getUserByEmailRequest);
                    if (getUserByEmailResponse.IsSuccess)
                    {
                        getUserByEmailResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        getUserByEmailResponseJson.IsSuccess = getUserByEmailResponse.IsSuccess;
                        getUserByEmailResponseJson.Message = getUserByEmailResponse.Message;
                        getUserByEmailResponseJson.UserDTO = getUserByEmailResponse.UserDTO;
                        getUserByEmailResponseJson.FriendsCount = getUserByEmailResponse.FriendsCount;
                        getUserByEmailResponseJson.RoutinesCount = getUserByEmailResponse.RoutinesCount;
                    }
                    else
                    {
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

        #region Create user
        [HttpPost("create-user")]
        public async Task<ActionResult<CreateUserResponseJson>> CreateUser([FromBody] CreateUserRequestJson createUserRequestJson)
        {
            CreateUserResponseJson createUserResponseJson = new CreateUserResponseJson();
            createUserResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (createUserRequestJson == null ||
                    string.IsNullOrEmpty(createUserRequestJson.Dni) ||
                    string.IsNullOrEmpty(createUserRequestJson.Username) ||
                    string.IsNullOrEmpty(createUserRequestJson.Email) ||
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
                        Email = createUserRequestJson.Email.Trim(),
                        Password = createUserRequestJson.Password.Trim(),
                        ConfirmPassword = createUserRequestJson.ConfirmPassword.Trim(),
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
            createGoogleUserResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (createGoogleUserRequestJson == null ||
                   string.IsNullOrEmpty(createGoogleUserRequestJson.Dni) ||
                   string.IsNullOrEmpty(createGoogleUserRequestJson.Username) ||
                   string.IsNullOrEmpty(createGoogleUserRequestJson.Email) ||
                   string.IsNullOrEmpty(createGoogleUserRequestJson.Password) ||
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
                        Email = createGoogleUserRequestJson.Email,
                        Password = createGoogleUserRequestJson.Email, // no se puede sacar la contraseña desde google, a si que se usa el email como contraseña
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

        #region Create admin
        [HttpPost("create-admin")]
        [Authorize(Roles = nameof(Role.ADMIN))]
        public async Task<ActionResult<CreateAdminResponseJson>> CreateAdmin([FromBody] CreateAdminRequestJson createAdminRequstJson)
        {
            CreateAdminResponseJson createAdminResponseJson = new CreateAdminResponseJson();
            createAdminResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (createAdminRequstJson == null ||
                 string.IsNullOrEmpty(createAdminRequstJson.Dni) ||
                 string.IsNullOrEmpty(createAdminRequstJson.Username) ||
                 string.IsNullOrEmpty(createAdminRequstJson.Email) ||
                 string.IsNullOrEmpty(createAdminRequstJson.Password) ||
                 string.IsNullOrEmpty(createAdminRequstJson.ConfirmPassword))
                {
                    createAdminResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA; 
                    createAdminResponseJson.IsSuccess = false;
                    createAdminResponseJson.Message = "invalid data, the data is null or empty";
                }
                else
                {
                    CreateGenericUserRequest createGenericUserRequest = new CreateGenericUserRequest
                    {
                        Dni = createAdminRequstJson.Dni,
                        Username = createAdminRequstJson.Username,
                        Surname = createAdminRequstJson.Surname,
                        Email = createAdminRequstJson.Email,
                        Password = createAdminRequstJson.Password,
                        ConfirmPassword = createAdminRequstJson.ConfirmPassword,
                        Role = Role.ADMIN
                    };

                    CreateUserResponse createUserResponse = await _userApplication.CreateUser(createGenericUserRequest);
                    if (createUserResponse.IsSuccess)
                    {
                        createAdminResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        createAdminResponseJson.IsSuccess = createUserResponse.IsSuccess;
                        createAdminResponseJson.Message = createUserResponse.Message;
                    }
                    else
                    {
                        createAdminResponseJson.IsSuccess = createUserResponse.IsSuccess;
                        createAdminResponseJson.Message = createUserResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                createAdminResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                createAdminResponseJson.IsSuccess = false;
                createAdminResponseJson.Message = $"unexpected error on UserController -> create-admin: {ex.Message}";
            }

            return Ok(createAdminResponseJson);
        }
        #endregion

        #region Update user
        [HttpPost("update-user")]
        public async Task<ActionResult<UpdateUserResponseJson>> UpdateUser([FromBody] UpdateUserRequestJson updateUserRequestJson)
        {
            UpdateUserResponseJson updateUserResponseJson = new UpdateUserResponseJson();
            updateUserResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (updateUserRequestJson == null ||
                    string.IsNullOrEmpty(updateUserRequestJson.OriginalEmail) ||
                    string.IsNullOrEmpty(updateUserRequestJson.DniToBeFound) ||
                    string.IsNullOrEmpty(updateUserRequestJson.Username) ||
                    string.IsNullOrEmpty(updateUserRequestJson.Email))
                {
                    updateUserResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    updateUserResponseJson.IsSuccess = false;
                    updateUserResponseJson.Message = "invalid data, the email is null or empty";
                }
                else
                {
                    UpdateUserRequest updateUserRequest = new UpdateUserRequest
                    {
                        OldEmail = updateUserRequestJson.OriginalEmail,
                        NewDni = updateUserRequestJson.DniToBeFound,
                        NewUsername = updateUserRequestJson.Username,
                        NewSurname = updateUserRequestJson.Surname,
                        NewEmail = updateUserRequestJson.Email,
                    };

                    UpdateUserResponse updateUserResponse = await _userApplication.UpdateUser(updateUserRequest);
                    if (updateUserResponse.IsSuccess)
                    {
                        updateUserResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        updateUserResponseJson.IsSuccess = updateUserResponse.IsSuccess;
                        updateUserResponseJson.Message = updateUserResponse.Message;
                        updateUserResponseJson.userDTO = updateUserResponse.UserDTO;
                    }
                    else
                    {
                        updateUserResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                        updateUserResponseJson.IsSuccess = updateUserResponse.IsSuccess;
                        updateUserResponseJson.Message = updateUserResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                updateUserResponseJson.IsSuccess = false;
                updateUserResponseJson.Message = $"unexpected error on UserController -> update-user: {ex.Message}";
            }

            return Ok(updateUserResponseJson);
        }
        #endregion

        #region Delete user
        [HttpPost("delete-user")]
        public async Task<ActionResult<DeleteUserResponseJson>> DeleteUser([FromBody] DeleteUserRequestJson deleteUserRequestJson)
        {
            DeleteUserResponseJson deleteUserResponseJson = new DeleteUserResponseJson();
            deleteUserResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (deleteUserRequestJson == null ||
                   string.IsNullOrEmpty(deleteUserRequestJson.Email))
                {
                    deleteUserResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA; 
                    deleteUserResponseJson.IsSuccess = false;
                    deleteUserResponseJson.Message = "invalid data the user email is null or empty";
                }
                else
                {
                    DeleteUserRequest deleteUserRequest = new DeleteUserRequest
                    {
                        Email = deleteUserRequestJson.Email
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
        public async Task<ActionResult<CreateNewPasswordResponseJson>> CreateNewPassword([FromBody] CreateNewPasswordRequestJson createNewPasswordRequestJson)
        {
            CreateNewPasswordResponseJson createNewPasswordResponseJson = new CreateNewPasswordResponseJson();
            createNewPasswordResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (createNewPasswordRequestJson == null ||
                    string.IsNullOrEmpty(createNewPasswordRequestJson.UserEmail))
                {
                    createNewPasswordResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA; 
                    createNewPasswordResponseJson.IsSuccess = false;
                    createNewPasswordResponseJson.Message = "invalid data, the email is null or empty";
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
        public async Task<ActionResult<ChangePasswordWithPasswordAndEmailResponseJson>> ChangePasswordWithPasswordAndEmail([FromBody] ChangePasswordWithPasswordAndEmailRequestJson changePasswordWithPasswordAndEmailRequestJson)
        {
            ChangePasswordWithPasswordAndEmailResponseJson changePasswordWithPasswordAndEmailResponseJson = new ChangePasswordWithPasswordAndEmailResponseJson();
            changePasswordWithPasswordAndEmailResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (changePasswordWithPasswordAndEmailRequestJson == null ||
                    string.IsNullOrEmpty(changePasswordWithPasswordAndEmailRequestJson.UserEmail) ||
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
        public async Task<ActionResult<GetUserProfileDetailsResponseJson>> GetUserProfileDetails([FromBody] GetUserProfileDetailsRequestJson getUserProfileDetailsRequestJson)
        {
            GetUserProfileDetailsResponseJson getUserProfileDetailsResponseJson = new GetUserProfileDetailsResponseJson();
            getUserProfileDetailsResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (getUserProfileDetailsRequestJson == null || 
                    string.IsNullOrEmpty(getUserProfileDetailsRequestJson.UserEmail))
                {
                    getUserProfileDetailsResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    getUserProfileDetailsResponseJson.IsSuccess = false;
                    getUserProfileDetailsResponseJson.Message = "invalid data, the data is null or empty";
                }
                else
                {
                    GetUserProfileDetailsRequest getUserProfileDetailsRequest = new GetUserProfileDetailsRequest
                    {
                        UserEmail = getUserProfileDetailsRequestJson.UserEmail,
                    };

                    GetUserProfileDetailsResponse getUserProfileDetailsResponse = await _userApplication.GetUserProfileDetails(getUserProfileDetailsRequest);
                    if (getUserProfileDetailsResponse.IsSuccess)
                    {
                        getUserProfileDetailsResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        getUserProfileDetailsResponseJson.IsSuccess = getUserProfileDetailsResponse.IsSuccess;
                        getUserProfileDetailsResponseJson.Message = getUserProfileDetailsResponse.Message;
                        getUserProfileDetailsResponseJson.Username = getUserProfileDetailsResponse.Username ;
                        getUserProfileDetailsResponseJson.InscriptionDate = getUserProfileDetailsResponse.InscriptionDate ;
                        getUserProfileDetailsResponseJson.RoutineCount = getUserProfileDetailsResponse.RoutineCount;
                    }
                    else
                    {
                        getUserProfileDetailsResponseJson.IsSuccess = getUserProfileDetailsResponse.IsSuccess;
                        getUserProfileDetailsResponseJson.Message = getUserProfileDetailsResponse.Message;
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