using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Application.DataTransferObject.Interchange.Step.GetDailyStepsInfo;
using RoutinesGymService.Application.DataTransferObject.Interchange.Step.GetStats;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Transversal.Common.Responses;
using RoutinesGymService.Transversal.JsonInterchange.Step.GetDailyStepsInfo;
using RoutinesGymService.Transversal.JsonInterchange.Step.GetStats;
using RoutinesGymService.Transversal.JsonInterchange.Step.SaveDailySteps;
using RoutinesGymService.Transversal.Security;

namespace RoutinesGymService.Service.WebApi.Controllers.App
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
        [ResourceAuthorization]
        public async Task<ActionResult<GetStepResponseJson>> GetSteps([FromBody] GetStepRequestJson getStepRequestJson)
        {
            GetStepResponseJson getStepResponseJson = new GetStepResponseJson();
            try
            {
                if (string.IsNullOrEmpty(getStepRequestJson.UserEmail))
                {
                    getStepResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    getStepResponseJson.IsSuccess = false;
                    getStepResponseJson.Message = "Invalid data, the email is null or empty";
                }
                else
                {
                    GetStepRequest getStatRequest = new GetStepRequest
                    {
                        UserEmail = getStepRequestJson.UserEmail
                    };

                    GetStepResponse getStepsResponse = await _stepApplication.GetSteps(getStatRequest);
                    if (getStepsResponse.IsSuccess)
                    {
                        getStepResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        getStepResponseJson.Steps = getStepsResponse.Stats;
                        getStepResponseJson.IsSuccess = getStepsResponse.IsSuccess;
                        getStepResponseJson.Message = getStepsResponse.Message;
                    }
                    else
                    {
                        getStepResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        getStepResponseJson.Steps = getStepsResponse.Stats;
                        getStepResponseJson.IsSuccess = getStepsResponse.IsSuccess;
                        getStepResponseJson.Message = getStepsResponse.Message;
                    }
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
        [ResourceAuthorization]
        public async Task<ActionResult<GetDailyStepsInfoResponseJson>> GetDailyStepsInfo([FromBody] GetDailyStepsInfoRequestJson getDailyStepsInfoRequestJson)
        {
            GetDailyStepsInfoResponseJson getDailyStepsInfoResponseJson = new GetDailyStepsInfoResponseJson();
            try
            {
                if (string.IsNullOrEmpty(getDailyStepsInfoRequestJson.UserEmail) ||
                    getDailyStepsInfoRequestJson.DailySteps == -1 || 
                    getDailyStepsInfoRequestJson.Day == DateTime.MinValue)
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
                        getDailyStepsInfoResponseJson.IsSuccess = getDailyStepsInfoResponse.IsSuccess;
                        getDailyStepsInfoResponseJson.Message = getDailyStepsInfoResponse.Message;
                    }
                    else
                    {
                        getDailyStepsInfoResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        getDailyStepsInfoResponseJson.DailyStepsGoal = getDailyStepsInfoResponse.DailyStepsGoal;
                        getDailyStepsInfoResponseJson.DailySteps = getDailyStepsInfoResponse.DailySteps;
                        getDailyStepsInfoResponseJson.IsSuccess = getDailyStepsInfoResponse.IsSuccess;
                        getDailyStepsInfoResponseJson.Message = getDailyStepsInfoResponse.Message;
                    }
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
        [ResourceAuthorization]
        public async Task<ActionResult<SaveDailyStepsResponseJson>> SaveDailySteps([FromBody] SaveDailyStepsRequestJson saveDailyStepsRequestJson)
        {
            SaveDailyStepsResponseJson saveDailyStepsResponseJson = new SaveDailyStepsResponseJson();
            try
            {
                if (string.IsNullOrEmpty(saveDailyStepsRequestJson.UserEmail) ||
                    saveDailyStepsRequestJson.Steps == -1 ||
                    saveDailyStepsRequestJson.DailyStepsGoal == -1)
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
                    {
                        saveDailyStepsResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        saveDailyStepsResponseJson.IsSuccess = saveDailyStepsResponse.IsSuccess;
                        saveDailyStepsResponseJson.Message = saveDailyStepsResponse.Message;
                    }
                    else
                    {
                        saveDailyStepsResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        saveDailyStepsResponseJson.IsSuccess = saveDailyStepsResponse.IsSuccess;
                        saveDailyStepsResponseJson.Message = saveDailyStepsResponse.Message;
                    }
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