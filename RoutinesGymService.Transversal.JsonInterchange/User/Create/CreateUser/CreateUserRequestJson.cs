namespace RoutinesGymService.Transversal.JsonInterchange.User.Create.CreateUser
{
    public class CreateUserRequestJson
    {
        public string? Dni { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}