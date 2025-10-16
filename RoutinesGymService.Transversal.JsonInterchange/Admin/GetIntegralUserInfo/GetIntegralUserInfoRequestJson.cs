namespace RoutinesGymService.Transversal.JsonInterchange.Admin.GetIntegralUserInfo
{
    public class GetIntegralUserInfoRequestJson
    {
        public long UserId { get; set; }
        public string MasterKey { get; set; } = string.Empty;
    }
}