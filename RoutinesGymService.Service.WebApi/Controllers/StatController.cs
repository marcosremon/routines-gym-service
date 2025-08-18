using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Application.DataTransferObject.Interchange.Stat.GetStats;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Transversal.Common;
using RoutinesGymService.Transversal.JsonInterchange.Stat.GetStats;

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

        #region GetStats
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
    }
}