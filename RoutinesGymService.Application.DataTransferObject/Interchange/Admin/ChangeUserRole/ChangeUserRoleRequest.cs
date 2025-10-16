namespace RoutinesGymService.Application.DataTransferObject.Interchange.Admin.ChangeUserRole
{
    public class ChangeUserRoleRequest
    {
        public string UserEmail { get; set; } = string.Empty;
        public string OldRole { get; set; } = string.Empty;
        public string NewRole { get; set; } = string.Empty;
    }
}