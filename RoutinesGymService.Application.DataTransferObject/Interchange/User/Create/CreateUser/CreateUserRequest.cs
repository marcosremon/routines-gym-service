namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateUser
{
    public class CreateUserRequest
    {
        public string Dni { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}