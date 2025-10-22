namespace RoutinesGymService.Transversal.JsonInterchange.User.UpdateUser
{
    public class UpdateUserRequestJson
    {
        public string OriginalEmail { get; set; } = string.Empty;
        public string DniToBeFound { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}