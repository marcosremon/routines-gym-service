namespace RoutinesGymService.Transversal.JsonInterchange.Admin.ChangeUserPassword
{
    public class ChangeUserPasswordRequestJson
    {
        public string UserEmail { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}