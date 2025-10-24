using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Application.DataTransferObject.SplitDay.UpdateSplitDay;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Transversal.Common.Responses;
using RoutinesGymService.Transversal.JsonInterchange.SplitDay.UpdateSplitDay;
using RoutinesGymService.Transversal.Security.SecurityFilters;

namespace RoutinesGymService.Service.WebApi.Controllers.App
{
    [ApiController]
    [Route("split-day")]
    public class SplitDayController : ControllerBase
    {
        private readonly ISplitDayApplication _splitDayApplication;

        public SplitDayController(ISplitDayApplication splitDayApplication)
        {
            _splitDayApplication = splitDayApplication;
        }

        #region Update split day
        [HttpPost("update-split-day")]
        [JwtValidationFilter]
        [ResourceAuthorizationFilter]
        public async Task<ActionResult<UpdateSplitDayResponseJson>> UpdateSplitDay([FromBody] UpdateSplitDayRequestJson updateSplitDayRequestJson)
        {
            UpdateSplitDayResponseJson updateSplitDayResponseJson = new UpdateSplitDayResponseJson();
            try
            {
                if (string.IsNullOrEmpty(updateSplitDayRequestJson.RoutineName) ||
                    string.IsNullOrEmpty(updateSplitDayRequestJson.UserEmail))
                {
                    updateSplitDayResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    updateSplitDayResponseJson.IsSuccess = false;
                    updateSplitDayResponseJson.Message = "invalid data, the email or the routine id is null or empty";
                }
                else
                {
                    UpdateSplitDayRequest actualizarSplitDayRequest = new UpdateSplitDayRequest
                    {
                        RoutineName = updateSplitDayRequestJson.RoutineName,
                        UserEmail = updateSplitDayRequestJson.UserEmail,
                        AddDays = updateSplitDayRequestJson.AddDays,
                        DeleteDays = updateSplitDayRequestJson.DeleteDays
                    };

                    UpdateSplitDayResponse updateSplitDayResponse = await _splitDayApplication.UpdateSplitDay(actualizarSplitDayRequest);
                    if (updateSplitDayResponse.IsSuccess)
                    {
                        updateSplitDayResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        updateSplitDayResponseJson.UserDto = updateSplitDayResponse.UserDto;
                        updateSplitDayResponseJson.IsSuccess = updateSplitDayResponse.IsSuccess;
                        updateSplitDayResponseJson.Message = updateSplitDayResponse.Message;
                    }
                    else
                    {
                        updateSplitDayResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;
                        updateSplitDayResponseJson.UserDto = updateSplitDayResponse.UserDto;
                        updateSplitDayResponseJson.IsSuccess = updateSplitDayResponse.IsSuccess;
                        updateSplitDayResponseJson.Message = updateSplitDayResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                updateSplitDayResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                updateSplitDayResponseJson.IsSuccess = false;
                updateSplitDayResponseJson.Message = $"unexpected error on SplitDayController -> update-split-day: {ex.Message}";
            }

            return Ok(updateSplitDayResponseJson);
        }
        #endregion
    }
}