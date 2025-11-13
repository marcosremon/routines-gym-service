namespace RoutinesGymService.Transversal.JsonInterchange.Admin.AddUserToBlackList
{
    public class AddUserToBlackListRequestJson
    {
        public long UserId { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}