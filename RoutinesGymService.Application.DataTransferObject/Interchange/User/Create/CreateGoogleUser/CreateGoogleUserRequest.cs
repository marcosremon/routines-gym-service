namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateGoogleUser
{
    public class CreateGoogleUserRequest
    {
        public string? Dni { get; set; }
        public string? Username { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}