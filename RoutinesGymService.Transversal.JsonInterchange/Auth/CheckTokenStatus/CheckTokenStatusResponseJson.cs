using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Transversal.JsonInterchange.Auth.CheckTokenStatus
{
    public class CheckTokenStatusResponseJson : BaseResponseJson
    {
        public bool IsValid { get; set; } = false;
    }
}