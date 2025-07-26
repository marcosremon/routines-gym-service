using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Transversal.JsonInterchange.Auth.LoginWeb
{
    public class LoginWebResponseJson : BaseResponseJson
    {
        public string BearerToken { get; set; } = string.Empty;
    }
}