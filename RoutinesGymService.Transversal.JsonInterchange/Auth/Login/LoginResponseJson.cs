using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Auth.Login
{
    public class LoginResponseJson : BaseResponseJson
    {
        public string BearerToken { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } 
    }
}