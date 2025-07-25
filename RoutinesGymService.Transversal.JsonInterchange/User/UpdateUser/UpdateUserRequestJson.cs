namespace RoutinesGymService.Transversal.JsonInterchange.User.UpdateUser
{
    public class UpdateUserRequestJson
    {
        public string? OriginalEmail { get; set; }
        public string? DniToBeFound { get; set; }
        public string? Username { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
    }
}