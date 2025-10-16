using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.AddNewUserFriend;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.DeleteFriend;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.GetAllUserFriends;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Transversal.Common.Responses;
using RoutinesGymService.Transversal.JsonInterchange.Friend.AddNewUserFriend;
using RoutinesGymService.Transversal.JsonInterchange.Friend.DeleteFriend;
using RoutinesGymService.Transversal.JsonInterchange.Friend.GetAllUserFriends;
using RoutinesGymService.Transversal.Security;
using System.Security.Claims;

namespace RoutinesGymService.Service.WebApi.Controllers.App
{
    [ApiController]
    [Route("friend")]
    public class FriendController : ControllerBase
    {
        private readonly IFriendApplication _friendApplication;

        public FriendController(IFriendApplication friendApplication)
        {
            _friendApplication = friendApplication;
        }

        #region Get all user friends 
        [HttpPost("get-all-user-friends")]
        [Authorize]
        [ResourceAuthorization]
        public async Task<ActionResult<GetAllUserFriendsResponseJson>> GetAllUserFriends([FromBody] GetAllUserFriendsRequestJson getAllUserFriendsRequestJson)
        {
            GetAllUserFriendsResponseJson getAllUserFriendsResponseJson = new GetAllUserFriendsResponseJson();
            try
            {
                if (getAllUserFriendsRequestJson == null)
                {
                    getAllUserFriendsResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    getAllUserFriendsResponseJson.IsSuccess = false;
                    getAllUserFriendsResponseJson.Message = "invalid data, the email is null or empty";
                }
                else
                {
                    GetAllUserFriendsRequest getAllUserFriendsRequest = new GetAllUserFriendsRequest
                    {
                        UserEmail = getAllUserFriendsRequestJson.UserEmail
                    };

                    GetAllUserFriendsResponse getAllUserFriendsResponse = await _friendApplication.GetAllUserFriends(getAllUserFriendsRequest);
                    if (getAllUserFriendsResponse.IsSuccess)
                    {
                        getAllUserFriendsResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        getAllUserFriendsResponseJson.Friends = getAllUserFriendsResponse.Friends;
                        getAllUserFriendsResponseJson.IsSuccess = getAllUserFriendsResponse.IsSuccess;
                        getAllUserFriendsResponseJson.Message = getAllUserFriendsResponse.Message;
                    }
                    else
                    {
                        getAllUserFriendsResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        getAllUserFriendsResponseJson.Friends = getAllUserFriendsResponse.Friends;
                        getAllUserFriendsResponseJson.IsSuccess = getAllUserFriendsResponse.IsSuccess;
                        getAllUserFriendsResponseJson.Message = getAllUserFriendsResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                getAllUserFriendsResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                getAllUserFriendsResponseJson.IsSuccess = false;
                getAllUserFriendsResponseJson.Message = $"unexpected error on FriendController -> get-all-user-friends {ex.Message}";
            }

            return Ok(getAllUserFriendsResponseJson);
        }
        #endregion

        #region Add new user friend 
        [HttpPost("add-new-user-friend")]
        [Authorize]
        [ResourceAuthorization]
        public async Task<ActionResult<AddNewUserFriendResponseJson>> AddNewUserFriend([FromBody] AddNewUserFriendRequestJson addNewUserFriendRequestJson)
        {
            AddNewUserFriendResponseJson addNewUserFriendResponseJson = new AddNewUserFriendResponseJson();
            try
            {
                if (addNewUserFriendRequestJson == null ||
                    string.IsNullOrEmpty(addNewUserFriendRequestJson.FriendCode))
                {
                    addNewUserFriendResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    addNewUserFriendResponseJson.IsSuccess = false;
                    addNewUserFriendResponseJson.Message = "invalid data, the email or the friend code is null or empty";
                }
                else
                {
                    AddNewUserFriendRequest addNewUserFriendRequest = new AddNewUserFriendRequest
                    {
                        UserEmail = addNewUserFriendRequestJson.UserEmail,
                        FriendCode = addNewUserFriendRequestJson.FriendCode
                    };

                    AddNewUserFriendResponse addNewUserFriendResponse = await _friendApplication.AddNewUserFriend(addNewUserFriendRequest);
                    if (addNewUserFriendResponse.IsSuccess)
                    {
                        addNewUserFriendResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        addNewUserFriendResponseJson.IsSuccess = addNewUserFriendResponse.IsSuccess;
                        addNewUserFriendResponseJson.Message = addNewUserFriendResponse.Message;
                    }
                    else
                    {
                        addNewUserFriendResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        addNewUserFriendResponseJson.IsSuccess = addNewUserFriendResponse.IsSuccess;
                        addNewUserFriendResponseJson.Message = addNewUserFriendResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                addNewUserFriendResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                addNewUserFriendResponseJson.IsSuccess = false;
                addNewUserFriendResponseJson.Message = $"unexpected error on FriendController -> add-new-user-friend: {ex.Message}";
            }

            return Ok(addNewUserFriendResponseJson);
        }
        #endregion

        #region Delete friend
        [HttpPost("delete-friend")]
        [Authorize]
        [ResourceAuthorization]
        public async Task<ActionResult<DeleteFriendResponseJson>> DeleteFriend([FromBody] DeleteFriendRequestJson deleteFriendRequestJson)
        {
            DeleteFriendResponseJson deleteFriendResponseJson = new DeleteFriendResponseJson();
            try
            {
                if (deleteFriendRequestJson == null ||
                    string.IsNullOrEmpty(deleteFriendRequestJson.UserEmail) ||
                    string.IsNullOrEmpty(deleteFriendRequestJson.FriendEmail))
                {
                    deleteFriendResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    deleteFriendResponseJson.IsSuccess = false;
                    deleteFriendResponseJson.Message = "invalid data, the email or the friend email is null or empty";
                }
                else
                {
                    DeleteFriendRequest deleteFriendRequest = new DeleteFriendRequest
                    {
                        UserEmail = deleteFriendRequestJson.UserEmail,
                        FriendEmail = deleteFriendRequestJson.FriendEmail
                    };

                    DeleteFriendResponse deleteFriendResponse = await _friendApplication.DeleteFriend(deleteFriendRequest);
                    if (deleteFriendResponse.IsSuccess)
                    {
                        deleteFriendResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        deleteFriendResponseJson.IsSuccess = deleteFriendResponse.IsSuccess;
                        deleteFriendResponseJson.Message = deleteFriendResponse.Message;
                    }
                    else
                    {
                        deleteFriendResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        deleteFriendResponseJson.IsSuccess = deleteFriendResponse.IsSuccess;
                        deleteFriendResponseJson.Message = deleteFriendResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                deleteFriendResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                deleteFriendResponseJson.IsSuccess = false;
                deleteFriendResponseJson.Message = $"unexpected error on FriendController -> delete-friend {ex.Message}";
            }

            return Ok(deleteFriendResponseJson);
        }
        #endregion
    }
}