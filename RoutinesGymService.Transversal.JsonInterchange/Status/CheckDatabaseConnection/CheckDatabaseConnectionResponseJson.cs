using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Status.CheckDatabaseConnection
{
    public class CheckDatabaseConnectionResponseJson
    {
        public ResponseCodesJson Status { get; set; }
        public string DatabaseConnection { get; set; } = string.Empty;
    }
}
