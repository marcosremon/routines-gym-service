namespace RoutinesGymService.Transversal.JsonInterchange.Admin.ChangeUserRole
{
    public class ChangeUserRoleRequestJson
    {
        public string UserEmail { get; set; } = string.Empty;
        public string OldRole { get; set; } = string.Empty;
        public string NewRole { get; set; } = string.Empty;
    }
}