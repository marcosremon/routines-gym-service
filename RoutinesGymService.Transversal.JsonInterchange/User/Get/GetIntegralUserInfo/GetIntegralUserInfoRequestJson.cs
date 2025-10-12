namespace RoutinesGymService.Transversal.JsonInterchange.User.Get.GetIntegralUserInfo
{
    public class GetIntegralUserInfoRequestJson
    {
        public long UserId { get; set; }
        public string MasterKey { get; set; } = string.Empty;
    }
}