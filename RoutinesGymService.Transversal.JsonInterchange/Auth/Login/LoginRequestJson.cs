namespace RoutinesGymService.Transversal.JsonInterchange.Auth.Login
{
    public class LoginRequestJson
    {
        public string UserEmail { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;    
        public string MobileGuid { get; set; } = string.Empty;    
    }
}