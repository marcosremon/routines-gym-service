namespace RoutinesGymService.Application.DataTransferObject.Interchange.Admin.ChangeUserPassword
{
    public class ChangeUserPasswordRequest
    {
        public string UserEmail { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}