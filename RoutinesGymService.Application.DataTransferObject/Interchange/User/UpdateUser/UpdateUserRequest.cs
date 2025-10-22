namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.UpdateUser
{
    public class UpdateUserRequest
    {
        public string OldEmail { get; set; } = string.Empty;
        public string NewEmail { get; set; } = string.Empty;
        public string NewDni { get; set; } = string.Empty;
        public string NewUsername { get; set; } = string.Empty;
        public string NewSurname { get; set; } = string.Empty;
    }
}