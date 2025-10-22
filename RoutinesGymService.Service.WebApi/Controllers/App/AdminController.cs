using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.ChangeUserRole;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetBlacklistedUsers;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetIntegralUserInfo;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetIntegralUsers;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetUsers;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetUsersByRole;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.RemoveUserFromBlackList;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.AddUserToBlackList;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateGenericUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateUser;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Domain.Model.Enums;
using RoutinesGymService.Transversal.Common.Responses;
using RoutinesGymService.Transversal.JsonInterchange.Admin.AddUserToBlackList;
using RoutinesGymService.Transversal.JsonInterchange.Admin.ChangeUserRole;
using RoutinesGymService.Transversal.JsonInterchange.Admin.CreateAdmin;
using RoutinesGymService.Transversal.JsonInterchange.Admin.GetBlacklistedUsers;
using RoutinesGymService.Transversal.JsonInterchange.Admin.GetIntegralUserInfo;
using RoutinesGymService.Transversal.JsonInterchange.Admin.GetIntegralUsers;
using RoutinesGymService.Transversal.JsonInterchange.Admin.GetUsers;
using RoutinesGymService.Transversal.JsonInterchange.Admin.GetUsersByRole;
using RoutinesGymService.Transversal.JsonInterchange.Admin.RemoveUserFromBlackList;

namespace RoutinesGymService.Service.WebApi.Controllers.App
{
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {

        private readonly IUserApplication _userApplication;
        private readonly IAdminApplication _adminApplication;

        public AdminController(IUserApplication userApplication, IAdminApplication adminApplication)
        {
            _userApplication = userApplication;
            _adminApplication = adminApplication;
        }

        #region Get users
        [HttpGet("get-users")]
        [Authorize(Roles = nameof(Role.ADMIN))]
        public async Task<ActionResult<GetUsersResponseJson>> GetUsers()
        {
            GetUsersResponseJson getUsersResponseJson = new GetUsersResponseJson();
            try
            {
                GetUsersResponse getUsersResponse = await _adminApplication.GetUsers();
                if (getUsersResponse.IsSuccess)
                {
                    getUsersResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                    getUsersResponseJson.UsersDto = getUsersResponse.UsersDto;
                    getUsersResponseJson.IsSuccess = getUsersResponse.IsSuccess;
                    getUsersResponseJson.Message = getUsersResponse.Message;
                }
                else
                {
                    getUsersResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                    getUsersResponseJson.UsersDto = getUsersResponse.UsersDto;
                    getUsersResponseJson.IsSuccess = getUsersResponse.IsSuccess;
                    getUsersResponseJson.Message = getUsersResponse.Message;
                }
            }
            catch (Exception ex)
            {
                getUsersResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                getUsersResponseJson.IsSuccess = false;
                getUsersResponseJson.Message = $"unexpected error on AdminController -> get-users {ex.Message}";
            }

            return Ok(getUsersResponseJson);
        }
        #endregion

        #region Get integral users
        [HttpPost("get-integral-users")]
        [Authorize(Roles = nameof(Role.ADMIN))]
        public async Task<ActionResult<GetIntegralUsersResponseJson>> GetIntegralUsers([FromBody] GetIntegralUsersRequestJson getIntegralUsersRequestJson)
        {
            GetIntegralUsersResponseJson getIntegralUsersResponseJson = new GetIntegralUsersResponseJson();
            try
            {
                if (string.IsNullOrEmpty(getIntegralUsersRequestJson.MasterKey))
                {
                    getIntegralUsersResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    getIntegralUsersResponseJson.IsSuccess = false;
                    getIntegralUsersResponseJson.Message = $"Invalid data the data is null or empty";
                }
                else
                {
                    GetIntegralUsersRequest getIntegralUsersRequest = new GetIntegralUsersRequest
                    {
                        MasterKey = getIntegralUsersRequestJson.MasterKey,
                    };

                    GetIntegralUsersResponse getIntegralUsersResponse = await _adminApplication.GetIntegralUsers(getIntegralUsersRequest);
                    if (getIntegralUsersResponse.IsSuccess)
                    {
                        getIntegralUsersResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        getIntegralUsersResponseJson.UsersDto = getIntegralUsersResponse.UsersDto;
                        getIntegralUsersResponseJson.IsSuccess = getIntegralUsersResponse.IsSuccess;
                        getIntegralUsersResponseJson.Message = getIntegralUsersResponse.Message;
                    }
                    else
                    {
                        getIntegralUsersResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        getIntegralUsersResponseJson.UsersDto = getIntegralUsersResponse.UsersDto;
                        getIntegralUsersResponseJson.IsSuccess = getIntegralUsersResponse.IsSuccess;
                        getIntegralUsersResponseJson.Message = getIntegralUsersResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                getIntegralUsersResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                getIntegralUsersResponseJson.IsSuccess = false;
                getIntegralUsersResponseJson.Message = $"unexpected error on AdminController -> get-integral-users {ex.Message}";
            }

            return Ok(getIntegralUsersResponseJson);
        }
        #endregion

        #region Get integral user info
        [HttpPost("get-integral-user-info")]
        [Authorize(Roles = nameof(Role.ADMIN))]
        public async Task<ActionResult<GetIntegralUserInfoResponseJson>> GetIntegralUserInfo([FromBody] GetIntegralUserInfoRequestJson getIntegralUserInfoRequestJson)
        {
            GetIntegralUserInfoResponseJson getIntegralUserInfoResponseJson = new GetIntegralUserInfoResponseJson();
            try
            {
                if (getIntegralUserInfoRequestJson.UserId == -1)
                {
                    getIntegralUserInfoResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    getIntegralUserInfoResponseJson.IsSuccess = false;
                    getIntegralUserInfoResponseJson.Message = "invalid data, the email is null or empty";
                }
                else
                {
                    GetIntegralUserInfoRequest getIntegralUserInfoRequest = new GetIntegralUserInfoRequest
                    {
                        UserId = getIntegralUserInfoRequestJson.UserId,
                        MasterKey = getIntegralUserInfoRequestJson.MasterKey,
                    };

                    GetIntegralUserInfoResponse getIntegralUserInfoResponse = await _adminApplication.GetIntegralUserInfo(getIntegralUserInfoRequest);
                    if (getIntegralUserInfoResponse.IsSuccess)
                    {
                        getIntegralUserInfoResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        getIntegralUserInfoResponseJson.UserDto = getIntegralUserInfoResponse.UserDto;
                        getIntegralUserInfoResponseJson.IsSuccess = getIntegralUserInfoResponse.IsSuccess;
                        getIntegralUserInfoResponseJson.Message = getIntegralUserInfoResponse.Message;
                    }
                    else
                    {
                        getIntegralUserInfoResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        getIntegralUserInfoResponseJson.UserDto = getIntegralUserInfoResponse.UserDto;
                        getIntegralUserInfoResponseJson.IsSuccess = getIntegralUserInfoResponse.IsSuccess;
                        getIntegralUserInfoResponseJson.Message = getIntegralUserInfoResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                getIntegralUserInfoResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                getIntegralUserInfoResponseJson.IsSuccess = false;
                getIntegralUserInfoResponseJson.Message = $"unexpected error -> AdminController -> get-integral-user-info: {ex.Message}";
            }

            return Ok(getIntegralUserInfoResponseJson);
        }
        #endregion

        #region Create admin
        [HttpPost("create-admin")]
        [Authorize(Roles = nameof(Role.ADMIN))]
        public async Task<ActionResult<CreateAdminResponseJson>> CreateAdmin([FromBody] CreateAdminRequestJson createAdminRequstJson)
        {
            CreateAdminResponseJson createAdminResponseJson = new CreateAdminResponseJson();
            try
            {
                if (string.IsNullOrEmpty(createAdminRequstJson.Dni) ||
                    string.IsNullOrEmpty(createAdminRequstJson.Username) ||
                    string.IsNullOrEmpty(createAdminRequstJson.Email) ||
                    string.IsNullOrEmpty(createAdminRequstJson.SerialNumber) ||
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
                        SerialNumber = createAdminRequstJson.SerialNumber,
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
                        createAdminResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        createAdminResponseJson.IsSuccess = createUserResponse.IsSuccess;
                        createAdminResponseJson.Message = createUserResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                createAdminResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                createAdminResponseJson.IsSuccess = false;
                createAdminResponseJson.Message = $"unexpected error on AdminController -> create-admin: {ex.Message}";
            }

            return Ok(createAdminResponseJson);
        }
        #endregion

        #region Add user to blacklist
        [HttpPost("add-user-to-black-list")]
        [Authorize(Roles = nameof(Role.ADMIN))]
        public async Task<ActionResult<AddUserToBlackListResponseJson>> AddUserToBlackList([FromBody] AddUserToBlackListRequestJson addUserToBlackListRequestJson)
        {
            AddUserToBlackListResponseJson addUserToBlackListResponseJson = new AddUserToBlackListResponseJson();
            try
            {
                if (addUserToBlackListRequestJson.UserId == -1 ||
                    string.IsNullOrEmpty(addUserToBlackListRequestJson.SerialNumber) ||
                    string.IsNullOrEmpty(addUserToBlackListRequestJson.Description))
                {
                    addUserToBlackListResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    addUserToBlackListResponseJson.IsSuccess = false;
                    addUserToBlackListResponseJson.Message = "invalid data, the data is null or empty";
                }
                else
                {
                    AddUserToBlackListRequest addUserToBlackListRequest = new AddUserToBlackListRequest
                    {
                        SerialNumber = addUserToBlackListRequestJson.SerialNumber,
                        UserId = addUserToBlackListRequestJson.UserId,
                        Description = addUserToBlackListRequestJson.Description,
                    };

                    AddUserToBlackListResponse addUserToBlackListResponse = await _adminApplication.AddUserToBlackList(addUserToBlackListRequest);
                    if (addUserToBlackListResponse.IsSuccess)
                    {
                        addUserToBlackListResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        addUserToBlackListResponseJson.IsSuccess = addUserToBlackListResponse.IsSuccess;
                        addUserToBlackListResponseJson.Message = addUserToBlackListResponse.Message;
                    }
                    else
                    {
                        addUserToBlackListResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        addUserToBlackListResponseJson.IsSuccess = addUserToBlackListResponse.IsSuccess;
                        addUserToBlackListResponseJson.Message = addUserToBlackListResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                addUserToBlackListResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                addUserToBlackListResponseJson.IsSuccess = false;
                addUserToBlackListResponseJson.Message = $"unexpected error on AdminController -> add-user-to-black-list {ex.Message}";
            }

            return Ok(addUserToBlackListResponseJson);
        }
        #endregion

        #region Remove user from blacklist
        [HttpPost("remove-user-from-black-list")]
        [Authorize(Roles = nameof(Role.ADMIN))]
        public async Task<ActionResult<RemoveUserFromBlackListResponseJson>> RemoveUserFromBlackList([FromBody] RemoveUserFromBlackListRequestJson removeUserFromBlackListRequestJson)
        {
            RemoveUserFromBlackListResponseJson removeUserFromBlackListResponseJson = new RemoveUserFromBlackListResponseJson();
            try
            {
                if (string.IsNullOrEmpty(removeUserFromBlackListRequestJson.UserEmail))
                {
                    removeUserFromBlackListResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    removeUserFromBlackListResponseJson.IsSuccess = false;
                    removeUserFromBlackListResponseJson.Message = "invalid data, the data is null or empty";
                }
                else
                {
                    RemoveUserFromBlackListRequest removeUserFromBlackListRequest = new RemoveUserFromBlackListRequest
                    {
                        UserEmail = removeUserFromBlackListRequestJson.UserEmail,
                    };

                    RemoveUserFromBlackListResponse removeUserFromBlackListResponse = await _adminApplication.RemoveUserFromBlackList(removeUserFromBlackListRequest);
                    if (removeUserFromBlackListResponse.IsSuccess)
                    {
                        removeUserFromBlackListResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        removeUserFromBlackListResponseJson.IsSuccess = removeUserFromBlackListResponse.IsSuccess;
                        removeUserFromBlackListResponseJson.Message = removeUserFromBlackListResponse.Message;
                    }
                    else
                    {
                        removeUserFromBlackListResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        removeUserFromBlackListResponseJson.IsSuccess = removeUserFromBlackListResponse.IsSuccess;
                        removeUserFromBlackListResponseJson.Message = removeUserFromBlackListResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                removeUserFromBlackListResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                removeUserFromBlackListResponseJson.IsSuccess = false;
                removeUserFromBlackListResponseJson.Message = $"unexpected error on AdminController -> remove-user-from-black-list {ex.Message}";
            }

            return Ok(removeUserFromBlackListResponseJson);
        }
        #endregion

        #region Get blacklisted users
        [HttpPost("get-blacklisted-users")]
        [Authorize(Roles = nameof(Role.ADMIN))]
        public async Task<ActionResult<GetBlacklistedUsersResponseJson>> GetBlacklistedUsers([FromBody] GetBlacklistedUsersRequestJson getBlacklistedUsersRequestJson)
        {
            GetBlacklistedUsersResponseJson getBlacklistedUsersResponseJson = new GetBlacklistedUsersResponseJson();
            try
            {
                if (string.IsNullOrEmpty(getBlacklistedUsersRequestJson.MasterKey))
                {
                    getBlacklistedUsersResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    getBlacklistedUsersResponseJson.IsSuccess = false;
                    getBlacklistedUsersResponseJson.Message = "invalid data, the data is null or empty";
                }
                else
                {
                    GetBlacklistedUsersRequest getBlacklistedUsersRequest = new GetBlacklistedUsersRequest
                    {
                        MasterKey = getBlacklistedUsersRequestJson.MasterKey,
                    };

                    GetBlacklistedUsersResponse getBlacklistedUsersResponse = await _adminApplication.GetBlacklistedUsers(getBlacklistedUsersRequest);
                    if (getBlacklistedUsersResponse.IsSuccess)
                    {
                        getBlacklistedUsersResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        getBlacklistedUsersResponseJson.BlackListUsers = getBlacklistedUsersResponse.BlackListUsers;
                        getBlacklistedUsersResponseJson.IsSuccess = getBlacklistedUsersResponse.IsSuccess;
                        getBlacklistedUsersResponseJson.Message = getBlacklistedUsersResponse.Message;
                    }
                    else
                    {
                        getBlacklistedUsersResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        getBlacklistedUsersResponseJson.BlackListUsers = getBlacklistedUsersResponse.BlackListUsers;
                        getBlacklistedUsersResponseJson.IsSuccess = getBlacklistedUsersResponse.IsSuccess;
                        getBlacklistedUsersResponseJson.Message = getBlacklistedUsersResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                getBlacklistedUsersResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                getBlacklistedUsersResponseJson.IsSuccess = false;
                getBlacklistedUsersResponseJson.Message = $"unexpected error on AdminController -> get-blacklisted-users {ex.Message}";
            }

            return Ok(getBlacklistedUsersResponseJson);
        }
        #endregion

        #region Change user role
        [HttpPost("change-user-role")]
        [Authorize(Roles = nameof(Role.ADMIN))]
        public async Task<ActionResult<ChangeUserRoleResponseJson>> ChangeUserRole([FromBody] ChangeUserRoleRequestJson changeUserRoleRequestJson)
        {
            ChangeUserRoleResponseJson changeUserRoleResponseJson = new ChangeUserRoleResponseJson();
            try 
            {
                if (string.IsNullOrEmpty(changeUserRoleRequestJson.UserEmail) ||
                    string.IsNullOrEmpty(changeUserRoleRequestJson.OldRole) ||
                    string.IsNullOrEmpty(changeUserRoleRequestJson.NewRole))
                {
                    changeUserRoleResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    changeUserRoleResponseJson.IsSuccess = false;
                    changeUserRoleResponseJson.Message = "invalid data, the data is null or empty";
                }
                else
                {
                    ChangeUserRoleRequest changeUserRoleRequest = new ChangeUserRoleRequest
                    {
                        UserEmail = changeUserRoleRequestJson.UserEmail,
                        OldRole = changeUserRoleRequestJson.OldRole.ToLower(),
                        NewRole = changeUserRoleRequestJson.NewRole.ToLower(),
                    };

                    ChangeUserRoleResponse changeUserRoleResponse = await _adminApplication.ChangeUserRole(changeUserRoleRequest);
                    if (changeUserRoleResponse.IsSuccess)
                    {
                        changeUserRoleResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        changeUserRoleResponseJson.IsSuccess = changeUserRoleResponse.IsSuccess;
                        changeUserRoleResponseJson.Message = changeUserRoleResponse.Message;
                    }
                    else
                    {
                        changeUserRoleResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        changeUserRoleResponseJson.IsSuccess = changeUserRoleResponse.IsSuccess;
                        changeUserRoleResponseJson.Message = changeUserRoleResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                changeUserRoleResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                changeUserRoleResponseJson.IsSuccess = false;
                changeUserRoleResponseJson.Message = $"unexpected error on AdminController -> change-user-role {ex.Message}";
            }

            return Ok(changeUserRoleResponseJson);
        }
        #endregion

        #region Get users by role
        [HttpPost("get-users-by-role")]
        [Authorize(Roles = nameof(Role.ADMIN))]
        public async Task<ActionResult<GetUsersByRoleResponseJson>> GetUsersByRole([FromBody] GetUsersByRoleRequestJson getUsersByRoleRequestJson)
        {
            GetUsersByRoleResponseJson getUsersByRoleResponseJson = new GetUsersByRoleResponseJson();
            try
            {
                if (string.IsNullOrEmpty(getUsersByRoleRequestJson.Role))
                {
                    getUsersByRoleResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    getUsersByRoleResponseJson.IsSuccess = false;
                    getUsersByRoleResponseJson.Message = "invalid data, the data is null or empty";
                }
                else
                {
                    GetUsersByRoleRequest getUsersByRoleRequest = new GetUsersByRoleRequest
                    {
                        Role = getUsersByRoleRequestJson.Role.ToLower(),
                    };

                    GetUsersByRoleResponse getUsersByRoleResponse = await _adminApplication.GetUsersByRole(getUsersByRoleRequest);
                    if (getUsersByRoleResponse.IsSuccess)
                    {
                        getUsersByRoleResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        getUsersByRoleResponseJson.UsersByRole = getUsersByRoleResponse.UsersByRole;
                        getUsersByRoleResponseJson.IsSuccess = getUsersByRoleResponse.IsSuccess;
                        getUsersByRoleResponseJson.Message = getUsersByRoleResponse.Message;
                    }
                    else
                    {
                        getUsersByRoleResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        getUsersByRoleResponseJson.UsersByRole = getUsersByRoleResponse.UsersByRole;
                        getUsersByRoleResponseJson.IsSuccess = getUsersByRoleResponse.IsSuccess;
                        getUsersByRoleResponseJson.Message = getUsersByRoleResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                getUsersByRoleResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                getUsersByRoleResponseJson.IsSuccess = false;
                getUsersByRoleResponseJson.Message = $"unexpected error on AdminController -> get-users-by-role {ex.Message}";
            }

            return Ok(getUsersByRoleResponseJson);
        }
        #endregion
    }
}