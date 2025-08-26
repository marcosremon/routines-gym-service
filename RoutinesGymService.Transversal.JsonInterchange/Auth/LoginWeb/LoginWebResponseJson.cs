using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Auth.LoginWeb
{
    public class LoginWebResponseJson : BaseResponseJson
    {
        public string BearerToken { get; set; } = string.Empty;
    }
}