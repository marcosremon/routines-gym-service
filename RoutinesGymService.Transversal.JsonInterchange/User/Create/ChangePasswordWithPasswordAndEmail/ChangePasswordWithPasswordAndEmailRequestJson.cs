namespace RoutinesGymService.Transversal.JsonInterchange.User.Create.ChangePasswordWithPasswordAndEmail
{
    public class ChangePasswordWithPasswordAndEmailRequestJson
    {
        public string UserEmail { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}