namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.UpdateUser
{
    public class UpdateUserRequest
    {
        public string? OldEmail { get; set; }
        public string? NewEmail { get; set; }
        public string? NewDni { get; set; }
        public string? NewUsername { get; set; }
        public string? NewSurname { get; set; }
    }
}