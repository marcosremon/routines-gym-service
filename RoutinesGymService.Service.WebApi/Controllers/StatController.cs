using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Application.DataTransferObject.Interchange.Stat.GetDailyStepsInfo;
using RoutinesGymService.Application.DataTransferObject.Interchange.Stat.GetStats;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Transversal.Common.Responses;
using RoutinesGymService.Transversal.JsonInterchange.Stat.GetDailyStepsInfo;
using RoutinesGymService.Transversal.JsonInterchange.Stat.GetStats;
using RoutinesGymService.Transversal.JsonInterchange.Stat.SaveDailySteps;

namespace RoutinesGymService.Service.WebApi.Controllers
{
    [ApiController]
    [Route("stat")]
    public class StatController : ControllerBase
    {
        private readonly IStatApplication _statApplication;

        public StatController(IStatApplication statApplication)
        {
            _statApplication = statApplication;
        }

        #region Get stats
        [HttpPost("get-stats")]
        public async Task<ActionResult<GetStatsResponseJson>> GetStats([FromBody] GetStatRequestJson getStatRequestJson)
        {
            GetStatsResponseJson getStatsResponseJson = new GetStatsResponseJson();
            getStatsResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (string.IsNullOrEmpty(getStatRequestJson.UserEmail))
                {
                    getStatsResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    getStatsResponseJson.IsSuccess = false;
                    getStatsResponseJson.Message = "invalid data the user email is null or empty";
                }
                else
                {
                    GetStatRequest getStatRequest = new GetStatRequest
                    {
                        UserEmail = getStatRequestJson.UserEmail
                    };

                    GetStatsResponse getStatsResponse = await _statApplication.GetStats(getStatRequest);
                    if (getStatsResponse.IsSuccess)
                    {
                        getStatsResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        getStatsResponseJson.IsSuccess = getStatsResponse.IsSuccess;
                        getStatsResponseJson.Message = getStatsResponse.Message;
                        getStatsResponseJson.Stats = getStatsResponse.Stats;
                    }
                    else
                    {
                        getStatsResponseJson.IsSuccess = getStatsResponse.IsSuccess;
                        getStatsResponseJson.Message = getStatsResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                getStatsResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                getStatsResponseJson.IsSuccess = false;
                getStatsResponseJson.Message = $"unexpected error on StatController -> get-stats {ex.Message}";
            }

            return Ok(getStatsResponseJson);
        }
        #endregion

        #region Get daily steps info
        [HttpPost("get-daily-steps-info")]
        public async Task<ActionResult<GetDailyStepsInfoResponseJson>> GetDailyStepsInfo([FromBody] GetDailyStepsInfoRequestJson getDailyStepsInfoRequestJson)
        {
            GetDailyStepsInfoResponseJson getDailyStepsInfoResponseJson = new GetDailyStepsInfoResponseJson();
            getDailyStepsInfoResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (getDailyStepsInfoRequestJson.DailySteps == null || 
                    getDailyStepsInfoRequestJson.Day == null ||
                    string.IsNullOrEmpty(getDailyStepsInfoRequestJson.UserEmail))
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

                    GetDailyStepsInfoResponse getDailyStepsInfoResponse = await _statApplication.GetDailyStepsInfo(getDailyStepsInfoRequest);
                    if (getDailyStepsInfoResponse.IsSuccess)
                    {
                        getDailyStepsInfoResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        getDailyStepsInfoResponseJson.IsSuccess = getDailyStepsInfoResponse.IsSuccess;
                        getDailyStepsInfoResponseJson.Message = getDailyStepsInfoResponse.Message;
                        getDailyStepsInfoResponseJson.DailyStepsGoal = getDailyStepsInfoResponse.DailyStepsGoal;
                        getDailyStepsInfoResponseJson.DailySteps = getDailyStepsInfoResponse.DailySteps;
                    }
                    else
                    {
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
        public async Task<ActionResult<SaveDailyStepsResponseJson>> SaveDailySteps([FromBody] SaveDailyStepsRequestJson saveDailyStepsRequestJson)
        {
            SaveDailyStepsResponseJson saveDailyStepsResponseJson = new SaveDailyStepsResponseJson();
            saveDailyStepsResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
          
            try
            {
                if (saveDailyStepsRequestJson == null ||
                    saveDailyStepsRequestJson.Steps == null ||
                    saveDailyStepsRequestJson.DailyStepsGoal == null ||
                    string.IsNullOrEmpty(saveDailyStepsRequestJson.UserEmail))
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

                    SaveDailyStepsResponse saveDailyStepsResponse = await _statApplication.SaveDailySteps(saveDailyStepsRequest);
                    if (saveDailyStepsResponse.IsSuccess)
                    {
                        saveDailyStepsResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        saveDailyStepsResponseJson.IsSuccess = saveDailyStepsResponse.IsSuccess;
                        saveDailyStepsResponseJson.Message = saveDailyStepsResponse.Message;
                    }
                    else
                    {
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