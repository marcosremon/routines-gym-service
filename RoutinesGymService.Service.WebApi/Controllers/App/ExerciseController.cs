using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExercise;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExerciseProgress;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.DeleteExercise;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.GetAllExerciseProgress;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.GetExercisesByDayAndRoutineId;
using RoutinesGymService.Application.DataTransferObject.Interchange.Friend.GetAllUserFriends;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Transversal.Common.Responses;
using RoutinesGymService.Transversal.JsonInterchange.Exercise.AddExercise;
using RoutinesGymService.Transversal.JsonInterchange.Exercise.AddExerciseProgress;
using RoutinesGymService.Transversal.JsonInterchange.Exercise.DeleteExercise;
using RoutinesGymService.Transversal.JsonInterchange.Exercise.GetAllExerciseProgress;
using RoutinesGymService.Transversal.JsonInterchange.Exercise.GetExercisesByDayAndRoutineId;
using RoutinesGymService.Transversal.Security.SecurityFilters;
using System.Security.Claims;

namespace RoutinesGymService.Service.WebApi.Controllers.App
{
    [ApiController]
    [Route("exercise")]
    public class ExerciseController : ControllerBase
    {
        private readonly IExerciseApplication _exerciseApplication;
        private readonly IFriendApplication _friendApplication;

        public ExerciseController(IExerciseApplication exerciseApplication, IFriendApplication friendApplication)
        {
            _exerciseApplication = exerciseApplication;
            _friendApplication = friendApplication;
        }

        #region Add exercise progress
        [HttpPost("add-exercise-progress")]
        [Authorize]
        [JwtValidationFilter]
        [ResourceAuthorizationFilter]
        public async Task<ActionResult<AddExerciseProgressResponseJson>> AddExerciseProgress([FromBody] AddExerciseProgressRequestJson addExerciseRequestJson)
        {
            AddExerciseProgressResponseJson addExerciseAddExerciseProgressResponseJson = new AddExerciseProgressResponseJson();
            try
            {
                if (addExerciseRequestJson.RoutineId == -1 ||
                    addExerciseRequestJson.splitDayId == -1 ||
                    string.IsNullOrEmpty(addExerciseRequestJson.ExerciseName) ||
                    string.IsNullOrEmpty(addExerciseRequestJson.UserEmail))
                {
                    addExerciseAddExerciseProgressResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    addExerciseAddExerciseProgressResponseJson.IsSuccess = false;
                    addExerciseAddExerciseProgressResponseJson.Message = "invalid data, user email or routine id is null or empty";
                }
                else
                {
                    AddExerciseAddExerciseProgressRequest addExerciseRequest = new AddExerciseAddExerciseProgressRequest
                    {
                        ProgressList = addExerciseRequestJson.ProgressList,
                        UserEmail = addExerciseRequestJson.UserEmail,
                        RoutineId = addExerciseRequestJson.RoutineId,
                        splitDayId = addExerciseRequestJson.splitDayId,
                        ExerciseName = addExerciseRequestJson.ExerciseName
                    };
                    
                    AddExerciseAddExerciseProgressResponse addExerciseAddExerciseProgressResponse = await _exerciseApplication.AddExerciseProgress(addExerciseRequest);
                    if (addExerciseAddExerciseProgressResponse.IsSuccess)
                    {
                        addExerciseAddExerciseProgressResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        addExerciseAddExerciseProgressResponseJson.IsSuccess = addExerciseAddExerciseProgressResponse.IsSuccess;
                        addExerciseAddExerciseProgressResponseJson.Message = addExerciseAddExerciseProgressResponse.Message;
                    }
                    else
                    {
                        addExerciseAddExerciseProgressResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        addExerciseAddExerciseProgressResponseJson.IsSuccess = addExerciseAddExerciseProgressResponse.IsSuccess;
                        addExerciseAddExerciseProgressResponseJson.Message = addExerciseAddExerciseProgressResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                addExerciseAddExerciseProgressResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                addExerciseAddExerciseProgressResponseJson.IsSuccess = false;
                addExerciseAddExerciseProgressResponseJson.Message = $"unexpected error on ExerciseController -> add-exercise-progress {ex.Message}";
            }

            return Ok(addExerciseAddExerciseProgressResponseJson);
        }
        #endregion

        #region Delete exercise
        [HttpPost("delete-exercise")]
        [Authorize]
        [JwtValidationFilter]
        [ResourceAuthorizationFilter]
        public async Task<ActionResult<DeleteExerciseResponseJson>> DeleteExercise([FromBody] DeleteExerciseRequestJson deleteExerciseRequestJson)
        {
            DeleteExerciseResponseJson deleteExerciseResponseJson = new DeleteExerciseResponseJson();
            try
            {
                if (deleteExerciseRequestJson.RoutineId == -1 ||
                    string.IsNullOrEmpty(deleteExerciseRequestJson.UserEmail) ||
                    string.IsNullOrEmpty(deleteExerciseRequestJson.ExerciseName) ||
                    string.IsNullOrEmpty(deleteExerciseRequestJson.DayName))
                {
                    deleteExerciseResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    deleteExerciseResponseJson.IsSuccess = false;
                    deleteExerciseResponseJson.Message = "invalid data, user email, routine id, day name or exercise id is null or empty";
                }
                else
                {
                    DeleteExerciseRequest deleteExerciseRequest = new DeleteExerciseRequest
                    {
                        UserEmail = deleteExerciseRequestJson.UserEmail,
                        RoutineId = deleteExerciseRequestJson.RoutineId,
                        DayName = deleteExerciseRequestJson.DayName,
                        ExerciseName = deleteExerciseRequestJson.ExerciseName
                    };

                    DeleteExerciseResponse deleteExerciseResponse = await _exerciseApplication.DeleteExercise(deleteExerciseRequest);
                    if (deleteExerciseResponse.IsSuccess)
                    {
                        deleteExerciseResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        deleteExerciseResponseJson.IsSuccess = deleteExerciseResponse.IsSuccess;
                        deleteExerciseResponseJson.Message = deleteExerciseResponse.Message;
                    }
                    else
                    {
                        deleteExerciseResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        deleteExerciseResponseJson.IsSuccess = deleteExerciseResponse.IsSuccess;
                        deleteExerciseResponseJson.Message = deleteExerciseResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                deleteExerciseResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                deleteExerciseResponseJson.IsSuccess = false;
                deleteExerciseResponseJson.Message = $"unexpected error on ExerciseController -> delete-exercise {ex.Message}";
            }

            return Ok(deleteExerciseResponseJson);
        }
        #endregion

        #region Add exercise
        [HttpPost("add-exercise")]
        [Authorize]
        [JwtValidationFilter]
        [ResourceAuthorizationFilter]
        public async Task<ActionResult<AddExerciseResponseJson>> AddExercise([FromBody] AddExerciseRequestJson addExerciseRequestJson)
        {
            AddExerciseResponseJson addExerciseResponseJson = new AddExerciseResponseJson();
            try
            {
                if (string.IsNullOrEmpty(addExerciseRequestJson.RoutineName) ||
                    string.IsNullOrEmpty(addExerciseRequestJson.ExerciseName) ||
                    string.IsNullOrEmpty(addExerciseRequestJson.DayName) ||
                    string.IsNullOrEmpty(addExerciseRequestJson.UserEmail))
                {
                    addExerciseResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    addExerciseResponseJson.IsSuccess = false;
                    addExerciseResponseJson.Message = "invalid data, routine id, exercise name, day name or user email is null or empty";
                }
                else
                {
                    AddExerciseRequest addExerciseRequest = new AddExerciseRequest
                    {
                        RoutineName = addExerciseRequestJson.RoutineName,
                        ExerciseName = addExerciseRequestJson.ExerciseName,
                        DayName = addExerciseRequestJson.DayName,
                        UserEmail = addExerciseRequestJson.UserEmail,
                    };

                    AddExerciseResponse addExerciseResponse = await _exerciseApplication.AddExercise(addExerciseRequest);
                    if (addExerciseResponse.IsSuccess)
                    {
                        addExerciseResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        addExerciseResponseJson.IsSuccess = addExerciseResponse.IsSuccess;
                        addExerciseResponseJson.Message = addExerciseResponse.Message;
                    }
                    else
                    {
                        addExerciseResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        addExerciseResponseJson.IsSuccess = addExerciseResponse.IsSuccess;
                        addExerciseResponseJson.Message = addExerciseResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                addExerciseResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                addExerciseResponseJson.IsSuccess = false;
                addExerciseResponseJson.Message = $"unexpected error on ExerciseController -> add-exercise {ex.Message}";
            }

            return Ok(addExerciseResponseJson);
        }
        #endregion

        #region Get exercises by day and routine name
        [HttpPost("get-exercises-by-day-and-routine-name")]
        [Authorize]
        public async Task<ActionResult<GetExercisesByDayAndRoutineNameResponseJson>> GetExercisesByDayAndRoutineName([FromBody] GetExercisesByDayAndRoutineNameRequestJson getExercisesByDayNameAndRoutineNameRequestJson)
        {
            GetExercisesByDayAndRoutineNameResponseJson getExercisesByDayAndRoutineNameResponseJson = new GetExercisesByDayAndRoutineNameResponseJson();
            try
            {
                string? tokenEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(tokenEmail))
                {
                    getExercisesByDayAndRoutineNameResponseJson.ResponseCodeJson = ResponseCodesJson.UNAUTHORIZED;
                    getExercisesByDayAndRoutineNameResponseJson.IsSuccess = false;
                    getExercisesByDayAndRoutineNameResponseJson.Message = "UNAUTHORIZED";
                }
                else if (string.IsNullOrEmpty(getExercisesByDayNameAndRoutineNameRequestJson.RoutineName) ||
                         string.IsNullOrEmpty(getExercisesByDayNameAndRoutineNameRequestJson.DayName) ||
                         string.IsNullOrEmpty(getExercisesByDayNameAndRoutineNameRequestJson.UserEmail)) 
                {
                    getExercisesByDayAndRoutineNameResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    getExercisesByDayAndRoutineNameResponseJson.IsSuccess = false;
                    getExercisesByDayAndRoutineNameResponseJson.Message = "invalid data, routine name, day name or user email is null or empty";
                }
                else
                {
                    string requestedEmail = getExercisesByDayNameAndRoutineNameRequestJson.UserEmail;
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
                        getExercisesByDayAndRoutineNameResponseJson.ResponseCodeJson = ResponseCodesJson.UNAUTHORIZED;
                        getExercisesByDayAndRoutineNameResponseJson.IsSuccess = false;
                        getExercisesByDayAndRoutineNameResponseJson.Message = "UNAUTHORIZED";
                    }
                    else
                    {
                        GetExercisesByDayAndRoutineNameRequest getExercisesByDayNameAndRoutineNameRequest = new GetExercisesByDayAndRoutineNameRequest
                        {
                            RoutineName = getExercisesByDayNameAndRoutineNameRequestJson.RoutineName,
                            DayName = getExercisesByDayNameAndRoutineNameRequestJson.DayName,
                            UserEmail = requestedEmail
                        };

                        GetExercisesByDayAndRoutineNameResponse getExercisesByDayAndRoutineNameResponse = await _exerciseApplication.GetExercisesByDayAndRoutineName(getExercisesByDayNameAndRoutineNameRequest);
                        if (getExercisesByDayAndRoutineNameResponse.IsSuccess)
                        {
                            getExercisesByDayAndRoutineNameResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                            getExercisesByDayAndRoutineNameResponseJson.Exercises = getExercisesByDayAndRoutineNameResponse.Exercises;
                            getExercisesByDayAndRoutineNameResponseJson.PastProgress = getExercisesByDayAndRoutineNameResponse.PastProgress;
                            getExercisesByDayAndRoutineNameResponseJson.IsSuccess = getExercisesByDayAndRoutineNameResponse.IsSuccess;
                            getExercisesByDayAndRoutineNameResponseJson.Message = getExercisesByDayAndRoutineNameResponse.Message;
                        }
                        else
                        {
                            getExercisesByDayAndRoutineNameResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                            getExercisesByDayAndRoutineNameResponseJson.Exercises = getExercisesByDayAndRoutineNameResponse.Exercises;
                            getExercisesByDayAndRoutineNameResponseJson.PastProgress = getExercisesByDayAndRoutineNameResponse.PastProgress;
                            getExercisesByDayAndRoutineNameResponseJson.IsSuccess = getExercisesByDayAndRoutineNameResponse.IsSuccess;
                            getExercisesByDayAndRoutineNameResponseJson.Message = getExercisesByDayAndRoutineNameResponse.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                getExercisesByDayAndRoutineNameResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                getExercisesByDayAndRoutineNameResponseJson.IsSuccess = false;
                getExercisesByDayAndRoutineNameResponseJson.Message = $"unexpected error on ExerciseController -> get-exercises-by-day-and-routine-name: {ex.Message}";
            }

            return Ok(getExercisesByDayAndRoutineNameResponseJson);
        }
        #endregion

        #region Get all exercise progress
        [HttpPost("get-all-exercise-progress")]
        [Authorize]
        [JwtValidationFilter]
        [ResourceAuthorizationFilter]
        public async Task<ActionResult<GetAllExerciseProgressResponseJson>> GetAllExerciseProgress([FromBody] GetAllExerciseProgressRequestJson getAllExerciseProgressRequestJson)
        {
            GetAllExerciseProgressResponseJson getAllExerciseProgressResponseJson = new GetAllExerciseProgressResponseJson();
            try
            {
                if (string.IsNullOrEmpty(getAllExerciseProgressRequestJson.RoutineName) ||
                    string.IsNullOrEmpty(getAllExerciseProgressRequestJson.UserEmail) ||
                    string.IsNullOrEmpty(getAllExerciseProgressRequestJson.ExerciseName) ||
                    string.IsNullOrEmpty(getAllExerciseProgressRequestJson.DayName))
                {
                    getAllExerciseProgressResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    getAllExerciseProgressResponseJson.IsSuccess = false;
                    getAllExerciseProgressResponseJson.Message = "invalid data, entry parameters are null or empty";
                }
                else
                {
                    GetAllExerciseProgressRequest getAllExerciseProgressRequest = new GetAllExerciseProgressRequest
                    {
                        RoutineName = getAllExerciseProgressRequestJson.RoutineName,
                        DayName = getAllExerciseProgressRequestJson.DayName,
                        UserEmail = getAllExerciseProgressRequestJson.UserEmail,
                        ExerciseName = getAllExerciseProgressRequestJson.ExerciseName,
                    };

                    GetAllExerciseProgressResponse getAllExerciseProgressResponse = await _exerciseApplication.GetAllExerciseProgress(getAllExerciseProgressRequest);
                    if (getAllExerciseProgressResponse.IsSuccess)
                    {
                        getAllExerciseProgressResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        getAllExerciseProgressResponseJson.ExerciseProgressList = getAllExerciseProgressResponse.ExerciseProgressList;
                        getAllExerciseProgressResponseJson.IsSuccess = getAllExerciseProgressResponse.IsSuccess;
                        getAllExerciseProgressResponseJson.Message = getAllExerciseProgressResponse.Message;
                    }
                    else
                    {
                        getAllExerciseProgressResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        getAllExerciseProgressResponseJson.ExerciseProgressList = getAllExerciseProgressResponse.ExerciseProgressList;
                        getAllExerciseProgressResponseJson.IsSuccess = getAllExerciseProgressResponse.IsSuccess;
                        getAllExerciseProgressResponseJson.Message = getAllExerciseProgressResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                getAllExerciseProgressResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                getAllExerciseProgressResponseJson.IsSuccess = false;
                getAllExerciseProgressResponseJson.Message = $"unexpected error on ExerciseController -> get-all-exercise-progress {ex.Message}";
            }

            return Ok(getAllExerciseProgressResponseJson);
        }
        #endregion
    }
}