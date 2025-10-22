namespace RoutinesGymService.Application.DataTransferObject.Interchange.Admin.CreateAdmin
{
    public class CreateAdminRequest
    {
        public string Dni { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}