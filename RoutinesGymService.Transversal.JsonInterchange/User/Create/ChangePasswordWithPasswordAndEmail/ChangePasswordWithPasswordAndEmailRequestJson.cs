namespace RoutinesGymService.Transversal.JsonInterchange.User.Create.ChangePasswordWithPasswordAndEmail
{
    public class ChangePasswordWithPasswordAndEmailRequestJson
    {
        public string? UserEmail { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
    }
}