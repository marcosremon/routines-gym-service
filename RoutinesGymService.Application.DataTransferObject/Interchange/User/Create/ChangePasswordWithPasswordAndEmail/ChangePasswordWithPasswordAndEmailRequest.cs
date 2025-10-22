namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.ChangePasswordWithPasswordAndEmail
{
    public class ChangePasswordWithPasswordAndEmailRequest
    {
        public string UserEmail { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}