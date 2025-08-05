using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExercise;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExerciseProgress;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.DeleteExercise;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.GetExercisesByDayAndRoutineId;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.UpdateExercise;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Transversal.Common;
using RoutinesGymService.Transversal.JsonInterchange.Exercise.AddExercise;
using RoutinesGymService.Transversal.JsonInterchange.Exercise.AddExerciseProgress;
using RoutinesGymService.Transversal.JsonInterchange.Exercise.DeleteExercise;
using RoutinesGymService.Transversal.JsonInterchange.Exercise.GetExercisesByDayAndRoutineId;
using RoutinesGymService.Transversal.JsonInterchange.Exercise.UpdateExercise;

namespace RoutinesGymService.Service.WebApi.Controllers
{
    [ApiController]
    [Route("exercise")]
    public class ExerciseController : ControllerBase
    {
        private readonly IExerciseApplication _exerciseApplication;

        public ExerciseController(IExerciseApplication exerciseApplication)
        {
            _exerciseApplication = exerciseApplication;
        }

        #region Add exercise progress
        [HttpPost("add-exercise-progress")]
        public async Task<ActionResult<AddExerciseAddExerciseProgressResponseJson>> AddExerciseProgress([FromBody] AddExerciseAddExerciseProgressRequestJson addExerciseRequestJson)
        {
            AddExerciseAddExerciseProgressResponseJson addExerciseAddExerciseProgressResponseJson = new AddExerciseAddExerciseProgressResponseJson();
            addExerciseAddExerciseProgressResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (addExerciseRequestJson == null ||
                    addExerciseRequestJson.RoutineId == null ||
                    string.IsNullOrEmpty(addExerciseRequestJson.UserEmail) ||
                    string.IsNullOrEmpty(addExerciseRequestJson.DayName))
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
                        DayName = addExerciseRequestJson.DayName
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

        #region Update exercise
        [HttpPost("update-exercise")]
        public async Task<ActionResult<UpdateExerciseResponseJson>> UpdateExercise([FromBody] UpdateExerciseRequestJson updateExerciseRequestJson)
        {
            UpdateExerciseResponseJson updateExerciseResponseJson = new UpdateExerciseResponseJson();
            updateExerciseResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (updateExerciseRequestJson == null ||
                    updateExerciseRequestJson.UserId == null ||
                    updateExerciseRequestJson.RoutineId == null ||
                    updateExerciseRequestJson.DayName == 0 ||
                    string.IsNullOrEmpty(updateExerciseRequestJson.ExerciseName))
                {
                    updateExerciseResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    updateExerciseResponseJson.IsSuccess = false;
                    updateExerciseResponseJson.Message = "invalid data, user id, routine id, day name or exercise name is null or empty";
                }
                else
                {
                    UpdateExerciseRequest updateExerciseRequest = new UpdateExerciseRequest
                    {
                        UserId = updateExerciseRequestJson.UserId,
                        RoutineId = updateExerciseRequestJson.RoutineId,
                        DayName = updateExerciseRequestJson.DayName,
                        ExerciseName = updateExerciseRequestJson.ExerciseName,
                        Sets = updateExerciseRequestJson.Sets,
                        Reps = updateExerciseRequestJson.Reps,
                        Weight = updateExerciseRequestJson.Weight
                    };

                    UpdateExerciseResponse updateExerciseResponse = await _exerciseApplication.UpdateExercise(updateExerciseRequest);
                    if (updateExerciseResponse.IsSuccess)
                    {
                        updateExerciseResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        updateExerciseResponseJson.IsSuccess = updateExerciseResponse.IsSuccess;
                        updateExerciseResponseJson.Message = updateExerciseResponse.Message;
                        updateExerciseResponseJson.UserDTO = updateExerciseResponse.UserDTO;
                    }
                    else
                    {
                        updateExerciseResponseJson.IsSuccess = updateExerciseResponse.IsSuccess;
                        updateExerciseResponseJson.Message = updateExerciseResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                updateExerciseResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                updateExerciseResponseJson.IsSuccess = false;
                updateExerciseResponseJson.Message = $"unexpected error on ExerciseController -> update-exercise {ex.Message}";
            }

            return Ok(updateExerciseResponseJson);
        }
        #endregion

        #region Delete exercise
        [HttpPost("delete-exercise")]
        public async Task<ActionResult<DeleteExerciseResponseJson>> DeleteExercise([FromBody] DeleteExerciseRequestJson deleteExerciseRequestJson)
        {
            DeleteExerciseResponseJson deleteExerciseResponseJson = new DeleteExerciseResponseJson();
            deleteExerciseResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (deleteExerciseRequestJson == null ||
                    deleteExerciseRequestJson.RoutineId == null ||
                    deleteExerciseRequestJson.ExerciseId == null ||
                    string.IsNullOrEmpty(deleteExerciseRequestJson.UserEmail) ||
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
                        ExerciseId = deleteExerciseRequestJson.ExerciseId
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
        public async Task<ActionResult<AddExerciseResponseJson>> AddExercise([FromBody] AddExerciseRequestJson addExerciseRequestJson)
        {
            AddExerciseResponseJson addExerciseResponseJson = new AddExerciseResponseJson();
            addExerciseResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (addExerciseRequestJson == null ||
                    addExerciseRequestJson.RoutineId == null ||
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
                        RoutineId = addExerciseRequestJson.RoutineId.Value,
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

        #region Get exercises by day and routine id
        [HttpPost("get-exercises-by-day-and-routine-id")]
        public async Task<ActionResult<GetExercisesByDayAndRoutineIdResponseJson>> GetExercisesByDayAndRoutineId([FromBody] GetExercisesByDayAndRoutineIdRequestJson getExercisesByDayNameRequestJson)
        {
            GetExercisesByDayAndRoutineIdResponseJson getExercisesByDayAndRoutineIdResponseJson = new GetExercisesByDayAndRoutineIdResponseJson();
            getExercisesByDayAndRoutineIdResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (getExercisesByDayNameRequestJson == null ||
                    getExercisesByDayNameRequestJson.RoutineId == null ||
                    string.IsNullOrEmpty(getExercisesByDayNameRequestJson.DayName))
                {
                    getExercisesByDayAndRoutineIdResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    getExercisesByDayAndRoutineIdResponseJson.IsSuccess = false;
                    getExercisesByDayAndRoutineIdResponseJson.Message = "invalid data, routine id or day name is null or empty";
                }
                else
                {
                    GetExercisesByDayAndRoutineIdRequest getExercisesByDayNameRequest = new GetExercisesByDayAndRoutineIdRequest
                    {
                        RoutineId = getExercisesByDayNameRequestJson.RoutineId.Value,
                        DayName = getExercisesByDayNameRequestJson.DayName
                    };

                    GetExercisesByDayAndRoutineIdResponse getExercisesByDayAndRoutineIdResponse = await _exerciseApplication.GetExercisesByDayAndRoutineId(getExercisesByDayNameRequest);
                    if (getExercisesByDayAndRoutineIdResponse.IsSuccess)
                    {
                        getExercisesByDayAndRoutineIdResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        getExercisesByDayAndRoutineIdResponseJson.IsSuccess = getExercisesByDayAndRoutineIdResponse.IsSuccess;
                        getExercisesByDayAndRoutineIdResponseJson.Message = getExercisesByDayAndRoutineIdResponse.Message;
                        getExercisesByDayAndRoutineIdResponseJson.Exercises = getExercisesByDayAndRoutineIdResponse.Exercises;
                        getExercisesByDayAndRoutineIdResponseJson.PastProgress = getExercisesByDayAndRoutineIdResponse.PastProgress;
                    }
                    else
                    {
                        getExercisesByDayAndRoutineIdResponseJson.IsSuccess = getExercisesByDayAndRoutineIdResponse.IsSuccess;
                        getExercisesByDayAndRoutineIdResponseJson.Message = getExercisesByDayAndRoutineIdResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                getExercisesByDayAndRoutineIdResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                getExercisesByDayAndRoutineIdResponseJson.IsSuccess = false;
                getExercisesByDayAndRoutineIdResponseJson.Message = $"unexpected error on ExerciseController -> get-exercises-by-day-and-routine-id {ex.Message}";
            }

            return Ok(getExercisesByDayAndRoutineIdResponseJson);
        }
        #endregion
    }
}