using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Application.DataTransferObject.Interchange.Step.GetDailyStepsInfo;
using RoutinesGymService.Application.DataTransferObject.Interchange.Step.GetStats;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Transversal.Common.Responses;
using RoutinesGymService.Transversal.JsonInterchange.Step.GetDailyStepsInfo;
using RoutinesGymService.Transversal.JsonInterchange.Step.GetStats;
using RoutinesGymService.Transversal.JsonInterchange.Step.SaveDailySteps;
using System.Security.Claims;

namespace RoutinesGymService.Service.WebApi.Controllers
{
    [ApiController]
    [Route("step")]
    public class StepController : ControllerBase
    {
        private readonly IStepApplication _stepApplication;

        public StepController(IStepApplication stepApplication)
        {
            _stepApplication = stepApplication;
        }

        #region Get steps
        [HttpPost("get-steps")]
        [Authorize]
        public async Task<ActionResult<GetStepResponseJson>> GetStats([FromBody] GetStepRequestJson getStepRequestJson)
        {
            GetStepResponseJson getStepResponseJson = new GetStepResponseJson();
            getStepResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                string? tokenEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                bool isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "ADMIN";

                if (string.IsNullOrEmpty(tokenEmail))
                {
                    return Unauthorized();
                }
                else if (!isAdmin && tokenEmail != getStepRequestJson.UserEmail)
                {
                    return Unauthorized();
                }
                else
                {
                    GetStepRequest getStatRequest = new GetStepRequest
                    {
                        UserEmail = getStepRequestJson.UserEmail
                    };

                    GetStepResponse getStatsResponse = await _stepApplication.GetSteps(getStatRequest);
                    if (getStatsResponse.IsSuccess)
                    {
                        getStepResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        getStepResponseJson.Steps = getStatsResponse.Stats;
                    }

                    getStepResponseJson.IsSuccess = getStatsResponse.IsSuccess;
                    getStepResponseJson.Message = getStatsResponse.Message;
                }
            }
            catch (Exception ex)
            {
                getStepResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                getStepResponseJson.IsSuccess = false;
                getStepResponseJson.Message = $"unexpected error on StatController -> get-steps {ex.Message}";
            }

            return Ok(getStepResponseJson);
        }
        #endregion

        #region Get daily steps info
        [HttpPost("get-daily-steps-info")]
        [Authorize]
        public async Task<ActionResult<GetDailyStepsInfoResponseJson>> GetDailyStepsInfo([FromBody] GetDailyStepsInfoRequestJson getDailyStepsInfoRequestJson)
        {
            GetDailyStepsInfoResponseJson getDailyStepsInfoResponseJson = new GetDailyStepsInfoResponseJson();
            getDailyStepsInfoResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                string? tokenEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                bool isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "ADMIN";

                if (string.IsNullOrEmpty(tokenEmail))
                {
                    return Unauthorized();
                }
                else if (!isAdmin && tokenEmail != getDailyStepsInfoRequestJson.UserEmail)
                {
                    return Unauthorized();
                }
                else if (getDailyStepsInfoRequestJson.DailySteps == null || 
                    getDailyStepsInfoRequestJson.Day == null)
                {
                    getDailyStepsInfoResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    getDailyStepsInfoResponseJson.IsSuccess = false;
                    getDailyStepsInfoResponseJson.Message = "invalid data the user email is null or empty";
                }
                else
                {
                    GetDailyStepsInfoRequest getDailyStepsInfoRequest = new GetDailyStepsInfoRequest
                    {
                        UserEmail = getDailyStepsInfoRequestJson.UserEmail,
                        DailySteps = getDailyStepsInfoRequestJson.DailySteps,
                        Day = getDailyStepsInfoRequestJson.Day,
                    };

                    GetDailyStepsInfoResponse getDailyStepsInfoResponse = await _stepApplication.GetDailyStepsInfo(getDailyStepsInfoRequest);
                    if (getDailyStepsInfoResponse.IsSuccess)
                    {
                        getDailyStepsInfoResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        getDailyStepsInfoResponseJson.DailyStepsGoal = getDailyStepsInfoResponse.DailyStepsGoal;
                        getDailyStepsInfoResponseJson.DailySteps = getDailyStepsInfoResponse.DailySteps;
                    }

                    getDailyStepsInfoResponseJson.IsSuccess = getDailyStepsInfoResponse.IsSuccess;
                    getDailyStepsInfoResponseJson.Message = getDailyStepsInfoResponse.Message;
                }
            }
            catch (Exception ex)
            {
                getDailyStepsInfoResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                getDailyStepsInfoResponseJson.IsSuccess = false;
                getDailyStepsInfoResponseJson.Message = $"unexpected error on StatController -> get-daily-steps-info {ex.Message}";
            }

            return Ok(getDailyStepsInfoResponseJson);
        }
        #endregion

        #region Save daily steps
        [HttpPost("save-daily-steps")]
        [Authorize]
        public async Task<ActionResult<SaveDailyStepsResponseJson>> SaveDailySteps([FromBody] SaveDailyStepsRequestJson saveDailyStepsRequestJson)
        {
            SaveDailyStepsResponseJson saveDailyStepsResponseJson = new SaveDailyStepsResponseJson();
            saveDailyStepsResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
          
            try
            {
                string? tokenEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                bool isAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "ADMIN";

                if (string.IsNullOrEmpty(tokenEmail))
                {
                    return Unauthorized();
                }
                else if (!isAdmin && tokenEmail != saveDailyStepsRequestJson.UserEmail)
                {
                    return Unauthorized();
                }
                else if (saveDailyStepsRequestJson == null ||
                    saveDailyStepsRequestJson.Steps == null ||
                    saveDailyStepsRequestJson.DailyStepsGoal == null)
                {
                    saveDailyStepsResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    saveDailyStepsResponseJson.IsSuccess = false;
                    saveDailyStepsResponseJson.Message = "invalid data";
                }
                else
                {
                    SaveDailyStepsRequest saveDailyStepsRequest = new SaveDailyStepsRequest
                    {
                        Steps = saveDailyStepsRequestJson.Steps,
                        UserEmail = saveDailyStepsRequestJson.UserEmail,
                        DailyStepsGoal = saveDailyStepsRequestJson.DailyStepsGoal,
                    };

                    SaveDailyStepsResponse saveDailyStepsResponse = await _stepApplication.SaveDailySteps(saveDailyStepsRequest);
                    if (saveDailyStepsResponse.IsSuccess)
                        saveDailyStepsResponseJson.ResponseCodeJson = ResponseCodesJson.OK;

                    saveDailyStepsResponseJson.IsSuccess = saveDailyStepsResponse.IsSuccess;
                    saveDailyStepsResponseJson.Message = saveDailyStepsResponse.Message;
                }
            }
            catch (Exception ex)
            {
                saveDailyStepsResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                saveDailyStepsResponseJson.IsSuccess = false;
                saveDailyStepsResponseJson.Message = $"unexpected error on StatController -> save-daily-steps {ex.Message}";
            }

            return Ok(saveDailyStepsResponseJson);
        }
        #endregion
    }
}