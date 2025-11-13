using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Infraestructure.Persistence.Context;

namespace RoutinesGymService.Service.WebApi.Controllers.Server
{
    [ApiController]
    [Route("status")]
    public class ServerConnectionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServerConnectionController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region CheckServiceConnection
        [HttpGet("service")]
        public ActionResult<string> CheckServiceConnection()
        {
            return Ok("Ok");
        }
        #endregion

        #region CheckDatabaseConnection
        [HttpGet("database")]
        public async Task<ActionResult<string>> CheckDatabaseConnection()
        {
            string message = string.Empty;  
            try
            {
                bool databaseConnect = await _context.Database.CanConnectAsync();
                message = databaseConnect
                    ? "Database connection Ok" 
                    : "Cannot connect to the database";
            }
            catch (Exception ex)
            {
                message = $"Database connection error {ex.Message}";
            }

            return Ok(message);
        }
        #endregion
    }
}