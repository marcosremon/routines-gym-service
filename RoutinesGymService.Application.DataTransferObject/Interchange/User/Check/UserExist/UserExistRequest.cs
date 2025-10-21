namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.Check.UserExist
{
    public class UserExistRequest
    {
        public string Dni { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserEmailAndDni { get; set; } = string.Empty;
    }
}