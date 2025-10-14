using Microsoft.AspNetCore.Mvc;

namespace RoutinesGymService.Service.WebApi.Controllers
{
    [ApiController]
    [Route("check")]
    public class ServerConnectionController : ControllerBase
    {
        #region CheckServerConnection
        [HttpGet("CheckServerConnection")]
        public ActionResult<string> CheckServerConnection()
        {
            return Ok("Ok");
        }
        #endregion
    }
}