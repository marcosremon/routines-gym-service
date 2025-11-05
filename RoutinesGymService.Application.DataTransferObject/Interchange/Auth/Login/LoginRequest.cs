namespace RoutinesGymService.Application.DataTransferObject.Interchange.Auth.Login
{
    public class LoginRequest
    {
        public string UserEmail { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
        public string MobileGuid { get; set; } = string.Empty;
    }
}