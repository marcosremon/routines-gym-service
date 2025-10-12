namespace RoutinesGymService.Transversal.JsonInterchange.User.Create.CreateAdmin
{
    public class CreateAdminRequestJson
    {
        public string? Dni { get; set; }
        public string? Username { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public string? ConfirmPassword { get; set; }
    }
}