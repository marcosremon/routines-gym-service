using RoutinesGymService.Domain.Model.Enums;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateGenericUser
{
    public class CreateGenericUserRequest
    {
        public string Dni { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public Role Role { get; set; }
    }
}