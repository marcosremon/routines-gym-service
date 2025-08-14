using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Application.DataTransferObject.Interchange.Stat.GetStats;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUsers;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Application.UseCase;
using RoutinesGymService.Transversal.Common;
using RoutinesGymService.Transversal.JsonInterchange.Stat.GetStats;
using RoutinesGymService.Transversal.JsonInterchange.User.Get.GetUsers;

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
        public async Task<ActionResult<GetStatsResponseJson>> GetStats()
        {
            GetStatsResponseJson getStatsResponseJson = new GetStatsResponseJson();
            getStatsResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                GetStatsResponse getStatsResponse = await _statApplication.GetStats();
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