using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.GetAllUserFriends;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.CreateRoutine;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.DeleteRoutine;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetAllUserRoutines;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineById;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineStats;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.UpdateRoutine;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Transversal.Common.Responses;
using RoutinesGymService.Transversal.JsonInterchange.Routine.CreateRoutine;
using RoutinesGymService.Transversal.JsonInterchange.Routine.DeleteRoutine;
using RoutinesGymService.Transversal.JsonInterchange.Routine.GetAllUserRoutines;
using RoutinesGymService.Transversal.JsonInterchange.Routine.GetRoutineById;
using RoutinesGymService.Transversal.JsonInterchange.Routine.GetRoutineStats;
using RoutinesGymService.Transversal.JsonInterchange.Routine.UpdateRoutine;
using System.Security.Claims;

namespace RoutinesGymService.Service.WebApi.Controllers.App
{
    [ApiController]
    [Route("routine")]
    public class RoutineController : ControllerBase
    {
        private readonly IRoutineApplication _routineApplication;
        private readonly IFriendApplication _friendApplication;

        public RoutineController(IRoutineApplication routineApplication, IFriendApplication friendApplication)
        {
            _routineApplication = routineApplication;
            _friendApplication = friendApplication;
        }

        #region Create routine
        [HttpPost("create-routine")]
        [Authorize]
        public async Task<ActionResult<CreateRoutineResponseJson>> CreateRoutine([FromBody] CreateRoutineRequestJson createRoutineRequestJson)
        {
            CreateRoutineResponseJson createRoutineResponseJson = new CreateRoutineResponseJson();
            try
            {
                string? tokenEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                bool isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "ADMIN";

                if (string.IsNullOrEmpty(tokenEmail) || !isAdmin && tokenEmail != createRoutineRequestJson.UserEmail)
                {
                    createRoutineResponseJson.ResponseCodeJson = ResponseCodesJson.UNAUTHORIZED;
                    createRoutineResponseJson.IsSuccess = false;
                    createRoutineResponseJson.Message = "UNAUTHORIZED";
                }
                else if (createRoutineRequestJson == null ||
                    string.IsNullOrEmpty(createRoutineRequestJson.RoutineName))
                {
                    createRoutineResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    createRoutineResponseJson.IsSuccess = false;
                    createRoutineResponseJson.Message = "invalid data, the email or the routine name is null or empty";
                }
                else
                {
                    CreateRoutineRequest createRoutineRequest = new CreateRoutineRequest
                    {
                        UserEmail = createRoutineRequestJson.UserEmail,
                        RoutineName = createRoutineRequestJson.RoutineName,
                        RoutineDescription = createRoutineRequestJson.RoutineDescription,
                        SplitDays = createRoutineRequestJson.SplitDays,
                    };

                    CreateRoutineResponse createRoutineResponse = await _routineApplication.CreateRoutine(createRoutineRequest);
                    if (createRoutineResponse.IsSuccess)
                    {
                        createRoutineResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        createRoutineResponseJson.IsSuccess = createRoutineResponse.IsSuccess;
                        createRoutineResponseJson.Message = createRoutineResponse.Message;
                    }
                    else
                    {
                        createRoutineResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        createRoutineResponseJson.IsSuccess = createRoutineResponse.IsSuccess;
                        createRoutineResponseJson.Message = createRoutineResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                createRoutineResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                createRoutineResponseJson.IsSuccess = false;
                createRoutineResponseJson.Message = $"unexpected error on RoutineController -> create-controller: {ex.Message}";
            }

            return Ok(createRoutineResponseJson);
        }
        #endregion

        #region Update routine
        [HttpPost("update-routine")]
        public async Task<ActionResult<UpdateRoutineResponseJson>> UpdateRoutine([FromBody] UpdateRoutineRequestJson updateRoutineRequestJson)
        {
            UpdateRoutineResponseJson updateRoutineResponseJson = new UpdateRoutineResponseJson();
            try
            {
                if (updateRoutineRequestJson == null ||
                    updateRoutineRequestJson?.RoutineId == null)
                {
                    updateRoutineResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    updateRoutineResponseJson.IsSuccess = false;
                    updateRoutineResponseJson.Message = "invalid data, parameters are null or empty";
                }
                else
                {
                    UpdateRoutineRequest updateRoutineRequest = new UpdateRoutineRequest
                    {
                        RoutineId = updateRoutineRequestJson.RoutineId,
                        RoutineName = updateRoutineRequestJson.RoutineName,
                        RoutineDescription = updateRoutineRequestJson.RoutineDescription,
                        SplitDays = updateRoutineRequestJson.SplitDays
                    };

                    UpdateRoutineResponse updateRoutineResponse = await _routineApplication.UpdateUser(updateRoutineRequest);
                    if (updateRoutineResponse.IsSuccess)
                    {
                        updateRoutineResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        updateRoutineResponseJson.RoutineDTO = updateRoutineResponse.RoutineDTO;
                        updateRoutineResponseJson.IsSuccess = updateRoutineResponse.IsSuccess;
                        updateRoutineResponseJson.Message = updateRoutineResponse.Message;
                    }
                    else
                    {
                        updateRoutineResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        updateRoutineResponseJson.RoutineDTO = updateRoutineResponse.RoutineDTO;
                        updateRoutineResponseJson.IsSuccess = updateRoutineResponse.IsSuccess;
                        updateRoutineResponseJson.Message = updateRoutineResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                updateRoutineResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                updateRoutineResponseJson.IsSuccess = false;
                updateRoutineResponseJson.Message = $"unexpected error on RoutineController -> update-routine: {ex.Message}";
            }

            return Ok(updateRoutineResponseJson);
        }
        #endregion

        #region Delete routine
        [HttpPost("delete-routine")]
        [Authorize]
        public async Task<ActionResult<DeleteRoutineResponseJson>> DeleteRoutine([FromBody] DeleteRoutineRequestJson deleteRoutineRequestJson)
        {
            DeleteRoutineResponseJson deleteRoutineResponseJson = new DeleteRoutineResponseJson();
            try
            {
                string? tokenEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                bool isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "ADMIN";

                if (string.IsNullOrEmpty(tokenEmail) || !isAdmin && tokenEmail != deleteRoutineRequestJson.UserEmail)
                {
                    deleteRoutineResponseJson.ResponseCodeJson = ResponseCodesJson.UNAUTHORIZED;
                    deleteRoutineResponseJson.IsSuccess = false;
                    deleteRoutineResponseJson.Message = "UNAUTHORIZED";
                }
                else if (deleteRoutineRequestJson == null ||
                    string.IsNullOrEmpty(deleteRoutineRequestJson.RoutineName))
                {
                    deleteRoutineResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    deleteRoutineResponseJson.IsSuccess = false;
                    deleteRoutineResponseJson.Message = "invalid data, the email or the routine id is null or empty";
                }
                else
                {
                    DeleteRoutineRequest deleteRoutineRequest = new DeleteRoutineRequest
                    {
                        UserEmail = deleteRoutineRequestJson.UserEmail,
                        RoutineName = deleteRoutineRequestJson.RoutineName
                    };

                    DeleteRoutineResponse deleteRoutineResponse = await _routineApplication.DeleteRoutine(deleteRoutineRequest);
                    if (deleteRoutineResponse.IsSuccess)
                    {
                        deleteRoutineResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        deleteRoutineResponseJson.IsSuccess = deleteRoutineResponse.IsSuccess;
                        deleteRoutineResponseJson.Message = deleteRoutineResponse.Message;
                    }
                    else
                    {
                        deleteRoutineResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        deleteRoutineResponseJson.IsSuccess = deleteRoutineResponse.IsSuccess;
                        deleteRoutineResponseJson.Message = deleteRoutineResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                deleteRoutineResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                deleteRoutineResponseJson.IsSuccess = false;
                deleteRoutineResponseJson.Message = $"unexpected error on RoutineController -> delete-routine: {ex.Message}";
            }

            return Ok(deleteRoutineResponseJson);
        }
        #endregion

        #region Get all user routine
        [HttpPost("get-all-user-routines")]
        [Authorize]
        public async Task<ActionResult<GetAllUserRoutinesResponseJson>> GetAllUserRoutines([FromBody] GetAllUserRoutinesRequestJson getAllUserRoutinesRequestJson)
        {
            GetAllUserRoutinesResponseJson getAllUserRoutinesResponseJson = new GetAllUserRoutinesResponseJson();
            try
            {
                string? tokenEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(tokenEmail))
                {
                    getAllUserRoutinesResponseJson.ResponseCodeJson = ResponseCodesJson.UNAUTHORIZED;
                    getAllUserRoutinesResponseJson.IsSuccess = false;
                    getAllUserRoutinesResponseJson.Message = "UNAUTHORIZED";
                }
                else if (getAllUserRoutinesRequestJson == null ||
                    string.IsNullOrEmpty(getAllUserRoutinesRequestJson?.UserEmail))
                {
                    getAllUserRoutinesResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    getAllUserRoutinesResponseJson.IsSuccess = false;
                    getAllUserRoutinesResponseJson.Message = "invalid data, the email is null or empty";
                }
                else
                {
                    string requestedEmail = getAllUserRoutinesRequestJson.UserEmail;
                    bool isOwnProfile = requestedEmail == tokenEmail;

                    GetAllUserFriendsRequest getAllUserFriendsRequest = new GetAllUserFriendsRequest
                    {
                        UserEmail = tokenEmail
                    };

                    GetAllUserFriendsResponse getAllUserFriendsResponse = await _friendApplication.GetAllUserFriends(getAllUserFriendsRequest);
                    bool areFriends = getAllUserFriendsResponse.Friends?.Any(f => f.Email == requestedEmail) == true;
                    bool isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "ADMIN";

                    if (!isOwnProfile && !areFriends && !isAdmin)
                    {
                        getAllUserRoutinesResponseJson.ResponseCodeJson = ResponseCodesJson.UNAUTHORIZED;
                        getAllUserRoutinesResponseJson.IsSuccess = false;
                        getAllUserRoutinesResponseJson.Message = "UNAUTHORIZED";
                    }
                    else
                    {
                        GetAllUserRoutinesRequest getAllUserRoutinesRequest = new GetAllUserRoutinesRequest
                        {
                            UserEmail = requestedEmail,
                        };

                        GetAllUserRoutinesResponse getAllUserRoutinesResponse = await _routineApplication.GetAllUserRoutines(getAllUserRoutinesRequest);
                        if (getAllUserRoutinesResponse.IsSuccess)
                        {
                            getAllUserRoutinesResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                            getAllUserRoutinesResponseJson.Routines = getAllUserRoutinesResponse.Routines;
                            getAllUserRoutinesResponseJson.IsSuccess = getAllUserRoutinesResponse.IsSuccess;
                            getAllUserRoutinesResponseJson.Message = getAllUserRoutinesResponse.Message;
                        }
                        else
                        {
                            getAllUserRoutinesResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                            getAllUserRoutinesResponseJson.Routines = getAllUserRoutinesResponse.Routines;
                            getAllUserRoutinesResponseJson.IsSuccess = getAllUserRoutinesResponse.IsSuccess;
                            getAllUserRoutinesResponseJson.Message = getAllUserRoutinesResponse.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                getAllUserRoutinesResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                getAllUserRoutinesResponseJson.IsSuccess = false;
                getAllUserRoutinesResponseJson.Message = $"unexpected error on RoutineController -> get-all-user-routines: {ex.Message}";
            }

            return Ok(getAllUserRoutinesResponseJson);
        }
        #endregion

        #region Get routine stats
        [HttpPost("get-routine-stats")]
        [Authorize]
        public async Task<ActionResult<GetRoutineStatsResponseJson>> GetRoutineStats([FromBody] GetRoutineStatsRequestJson getRoutineStatsRequestJson)
        {
            GetRoutineStatsResponseJson getRoutineStatsResponseJson = new GetRoutineStatsResponseJson();
            try
            {
                string? tokenEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                bool isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "ADMIN";

                if (string.IsNullOrEmpty(tokenEmail) || !isAdmin && tokenEmail != getRoutineStatsRequestJson.UserEmail)
                {
                    getRoutineStatsResponseJson.ResponseCodeJson = ResponseCodesJson.UNAUTHORIZED;
                    getRoutineStatsResponseJson.IsSuccess = false;
                    getRoutineStatsResponseJson.Message = "UNAUTHORIZED";
                }
                else if (getRoutineStatsRequestJson == null)
                {
                    getRoutineStatsResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    getRoutineStatsResponseJson.IsSuccess = false;
                    getRoutineStatsResponseJson.Message = "invalid data, the email is null or empty";
                }
                else
                {
                    GetRoutineStatsRequest getRoutineStatsRequest = new GetRoutineStatsRequest
                    {
                        UserEmail = getRoutineStatsRequestJson.UserEmail,
                    };

                    GetRoutineStatsResponse getRoutineStatsResponse = await _routineApplication.GetRoutineStats(getRoutineStatsRequest);
                    if (getRoutineStatsResponse.IsSuccess)
                    {
                        getRoutineStatsResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        getRoutineStatsResponseJson.IsSuccess = getRoutineStatsResponse.IsSuccess;
                        getRoutineStatsResponseJson.Message = getRoutineStatsResponse.Message;
                        getRoutineStatsResponseJson.RoutinesCount = getRoutineStatsResponse.RoutinesCount;
                        getRoutineStatsResponseJson.ExercisesCount = getRoutineStatsResponse.ExercisesCount;
                        getRoutineStatsResponseJson.SplitsCount = getRoutineStatsResponse.SplitsCount;
                    }
                    else
                    {
                        getRoutineStatsResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        getRoutineStatsResponseJson.IsSuccess = getRoutineStatsResponse.IsSuccess;
                        getRoutineStatsResponseJson.Message = getRoutineStatsResponse.Message;
                        getRoutineStatsResponseJson.RoutinesCount = getRoutineStatsResponse.RoutinesCount;
                        getRoutineStatsResponseJson.ExercisesCount = getRoutineStatsResponse.ExercisesCount;
                        getRoutineStatsResponseJson.SplitsCount = getRoutineStatsResponse.SplitsCount;
                    }
                }
            }
            catch (Exception ex)
            {
                getRoutineStatsResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                getRoutineStatsResponseJson.IsSuccess = false;
                getRoutineStatsResponseJson.Message = $"unexpected error on RoutineController -> get-routine-stats: {ex.Message}";
            }

            return Ok(getRoutineStatsResponseJson);
        }
        #endregion

        #region Get routine by routine name
        [HttpPost("get-routine-by-routine-name")]
        [Authorize]
        public async Task<ActionResult<GetRoutineByRoutineNameResponseJson>> GetRoutineByRoutineName([FromBody] GetRoutineByRoutineNameRequestJson getRoutineByRoutineNameRequestJson)
        {
            GetRoutineByRoutineNameResponseJson getRoutineByRoutineNameResponseJson = new GetRoutineByRoutineNameResponseJson();
            try
            {
                string? tokenEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(tokenEmail))
                {
                    getRoutineByRoutineNameResponseJson.ResponseCodeJson = ResponseCodesJson.UNAUTHORIZED;
                    getRoutineByRoutineNameResponseJson.IsSuccess = false;
                    getRoutineByRoutineNameResponseJson.Message = "UNAUTHORIZED";
                }
                else if (getRoutineByRoutineNameRequestJson == null ||
                    string.IsNullOrEmpty(getRoutineByRoutineNameRequestJson.RoutineName) ||
                    string.IsNullOrEmpty(getRoutineByRoutineNameRequestJson.UserEmail))
                {
                    getRoutineByRoutineNameResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    getRoutineByRoutineNameResponseJson.IsSuccess = false;
                    getRoutineByRoutineNameResponseJson.Message = "invalid data, the routine id is null or empty";
                }
                else
                {
                    string requestedEmail = getRoutineByRoutineNameRequestJson.UserEmail;
                    bool isOwnProfile = requestedEmail == tokenEmail;

                    GetAllUserFriendsRequest getAllUserFriendsRequest = new GetAllUserFriendsRequest
                    {
                        UserEmail = tokenEmail
                    };

                    GetAllUserFriendsResponse getAllUserFriendsResponse = await _friendApplication.GetAllUserFriends(getAllUserFriendsRequest);
                    bool areFriends = getAllUserFriendsResponse.Friends?.Any(f => f.Email == requestedEmail) == true;
                    bool isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "ADMIN";

                    if (!isOwnProfile && !areFriends && !isAdmin)
                    {
                        getRoutineByRoutineNameResponseJson.ResponseCodeJson = ResponseCodesJson.UNAUTHORIZED;
                        getRoutineByRoutineNameResponseJson.IsSuccess = false;
                        getRoutineByRoutineNameResponseJson.Message = "UNAUTHORIZED";
                    }
                    else
                    {
                        GetRoutineByRoutineNameRequest getRoutineByRoutineNameRequest = new GetRoutineByRoutineNameRequest
                        {
                            RoutineName = getRoutineByRoutineNameRequestJson.RoutineName,
                            UserEmail = requestedEmail
                        };

                        GetRoutineByRoutineNameResponse getRoutineByRoutineNameResponse = await _routineApplication.GetRoutineByRoutineName(getRoutineByRoutineNameRequest);
                        if (getRoutineByRoutineNameResponse.IsSuccess)
                        {
                            getRoutineByRoutineNameResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                            getRoutineByRoutineNameResponseJson.RoutineDTO = getRoutineByRoutineNameResponse.RoutineDTO;
                            getRoutineByRoutineNameResponseJson.IsSuccess = getRoutineByRoutineNameResponse.IsSuccess;
                            getRoutineByRoutineNameResponseJson.Message = getRoutineByRoutineNameResponse.Message;
                        }
                        else
                        {
                            getRoutineByRoutineNameResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                            getRoutineByRoutineNameResponseJson.RoutineDTO = getRoutineByRoutineNameResponse.RoutineDTO;
                            getRoutineByRoutineNameResponseJson.IsSuccess = getRoutineByRoutineNameResponse.IsSuccess;
                            getRoutineByRoutineNameResponseJson.Message = getRoutineByRoutineNameResponse.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                getRoutineByRoutineNameResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                getRoutineByRoutineNameResponseJson.IsSuccess = false;
                getRoutineByRoutineNameResponseJson.Message = $"unexpected error on RoutineController -> get-routine-by-id: {ex.Message}";
            }

            return Ok(getRoutineByRoutineNameResponseJson);
        }
        #endregion
    }
}