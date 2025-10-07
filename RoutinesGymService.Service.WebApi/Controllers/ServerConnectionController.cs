using Microsoft.AspNetCore.Mvc;

namespace RoutinesGymService.Service.WebApi.Controllers
{
    [ApiController]
    public class ServerConnectionController : ControllerBase
    {
        #region CheckServerConnection
        [HttpGet("/api/CheckServerConnection")]
        public ActionResult<string> CheckServerConnection()
        {
            return Ok("Ok");
        }
        #endregion
    }
}