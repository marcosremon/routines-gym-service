namespace RoutinesGymService.Transversal.JsonInterchange.Admin.AddUserToBlackList
{
    public class AddUserToBlackListRequestJson
    {
        public long UserId { get; set; } = -1;
        public string SerialNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}