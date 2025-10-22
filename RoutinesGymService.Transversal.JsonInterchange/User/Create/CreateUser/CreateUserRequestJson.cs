namespace RoutinesGymService.Transversal.JsonInterchange.User.Create.CreateUser
{
    public class CreateUserRequestJson
    {
        public string Dni { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
    }
}