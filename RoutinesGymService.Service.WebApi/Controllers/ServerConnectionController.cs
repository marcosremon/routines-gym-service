using Microsoft.AspNetCore.Mvc;

namespace RoutinesGymService.Service.WebApi.Controllers
{
    [ApiController]
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