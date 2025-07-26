using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Application.DataTransferObject.SplitDay.DeleteSplitDay;
using RoutinesGymService.Application.DataTransferObject.SplitDay.UpdateSplitDay;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Domain.Model.Enums;
using RoutinesGymService.Transversal.Common;
using RoutinesGymService.Transversal.JsonInterchange.SplitDay.DeleteSplitDay;
using RoutinesGymService.Transversal.JsonInterchange.SplitDay.UpdateSplitDay;
using TFC.Transversal.Logs;

namespace RoutinesGymService.Controllers
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

        #region update-split-day
        [HttpPost("update-split-day")]
        public async Task<ActionResult<UpdateSplitDayResponseJson>> UpdateSplitDay([FromBody] UpdateSplitDayRequestJson updateSplitDayRequestJson)
        {
            UpdateSplitDayResponseJson updateSplitDayResponseJson = new UpdateSplitDayResponseJson();
            updateSplitDayResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (updateSplitDayRequestJson == null ||
                    updateSplitDayRequestJson.RoutineId == null ||
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
                        RoutineId = updateSplitDayRequestJson.RoutineId,
                        UserEmail = updateSplitDayRequestJson.UserEmail,
                        AddDays = updateSplitDayRequestJson.AddDays,
                        DeleteDays = updateSplitDayRequestJson.DeleteDays
                    };

                    UpdateSplitDayResponse updateSplitDayResponse = await _splitDayApplication.UpdateSplitDay(actualizarSplitDayRequest);
                    if (updateSplitDayResponse.IsSuccess)
                    {
                        updateSplitDayResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        updateSplitDayResponseJson.IsSuccess = updateSplitDayResponse.IsSuccess;
                        updateSplitDayResponseJson.Message = updateSplitDayResponse.Message;
                        updateSplitDayResponseJson.UserDTO = updateSplitDayResponse.UserDTO;
                    }
                    else
                    {
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

        #region delete-split-day
        [HttpPost("delete-split-day")]
        public async Task<ActionResult<DeleteSplitDayResponseJson>> DeleteSplitDay([FromBody] DeleteSplitDayRequestJson deleteSplitDayRequestJson)
        {
            DeleteSplitDayResponseJson deleteSplitDayResponseJson = new DeleteSplitDayResponseJson();
            deleteSplitDayResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (deleteSplitDayRequestJson == null ||
                    deleteSplitDayRequestJson.RoutineId == null ||
                    deleteSplitDayRequestJson.UserId == null)
                {
                    deleteSplitDayResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    deleteSplitDayResponseJson.IsSuccess = false;
                    deleteSplitDayResponseJson.Message = "invalid data, the routine id or user id is null";
                }
                else
                {
                    DeleteSplitDayRequest deleteSplitDayRequest = new DeleteSplitDayRequest
                    {
                        DayName = deleteSplitDayRequestJson.DayName,
                        RoutineId = deleteSplitDayRequestJson.RoutineId,
                        UserId = deleteSplitDayRequestJson.UserId
                    };

                    DeleteSplitDayResponse deleteSplitDayResponse = await _splitDayApplication.DeleteSplitDay(deleteSplitDayRequest);
                    if (deleteSplitDayResponse.IsSuccess)
                    {
                        deleteSplitDayResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        deleteSplitDayResponseJson.IsSuccess = deleteSplitDayResponse.IsSuccess;
                        deleteSplitDayResponseJson.Message = deleteSplitDayResponse.Message;
                    }
                    else
                    {
                        deleteSplitDayResponseJson.IsSuccess = deleteSplitDayResponse.IsSuccess;
                        deleteSplitDayResponseJson.Message = deleteSplitDayResponse.Message;
                    }
                }        
            }
            catch (Exception ex)
            {
                deleteSplitDayResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                deleteSplitDayResponseJson.IsSuccess = false;
                deleteSplitDayResponseJson.Message = $"unexpected error on SplitDayController -> delete-split-day: {ex.Message}";
            }

            return Ok(deleteSplitDayResponseJson);
        }
        #endregion
    }
}