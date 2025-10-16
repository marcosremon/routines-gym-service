using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Infraestructure.Persistence.Context;
using RoutinesGymService.Transversal.Common.Responses;
using RoutinesGymService.Transversal.JsonInterchange.Status.CheckDatabaseConnection;
using RoutinesGymService.Transversal.JsonInterchange.Status.CheckServiceConnection;

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
        public ActionResult<CheckServiceConnectionResponseJson> CheckServiceConnection()
        {
            CheckServiceConnectionResponseJson checkServiceConnectionResponseJson = new CheckServiceConnectionResponseJson
            {
                CheckServiceConnection = "Serice connection Ok"
            };

            return Ok(checkServiceConnectionResponseJson);
        }
        #endregion

        #region CheckDatabaseConnection
        [HttpGet("database")]
        public async Task<ActionResult<CheckDatabaseConnectionResponseJson>> CheckDatabaseConnection()
        {
            CheckDatabaseConnectionResponseJson checkDatabaseConnectionResponseJson = new CheckDatabaseConnectionResponseJson();
            try
            {
                bool canConnect = await _context.Database.CanConnectAsync();
                if (canConnect)
                {
                    checkDatabaseConnectionResponseJson.Status = ResponseCodesJson.OK;
                    checkDatabaseConnectionResponseJson.DatabaseConnection = "Database connection Ok";
                }
                else
                {
                    checkDatabaseConnectionResponseJson.Status = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                    checkDatabaseConnectionResponseJson.DatabaseConnection = "Cannot connect to the database";
                }
            }
            catch (Exception ex)
            {
                checkDatabaseConnectionResponseJson.Status = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                checkDatabaseConnectionResponseJson.DatabaseConnection = $"Database connection error {ex.Message}";
            }

            return Ok(checkDatabaseConnectionResponseJson);
        }
        #endregion
    }
}