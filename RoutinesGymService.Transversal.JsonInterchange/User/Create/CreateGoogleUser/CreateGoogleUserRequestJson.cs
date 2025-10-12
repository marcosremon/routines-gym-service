using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateGoogleUser;

namespace RoutinesGymService.Transversal.JsonInterchange.User.Create.CreateGoogleUser
{
    public class CreateGoogleUserRequestJson
    {
        public string? Dni { get; set; }
        public string? Username { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
    }
}