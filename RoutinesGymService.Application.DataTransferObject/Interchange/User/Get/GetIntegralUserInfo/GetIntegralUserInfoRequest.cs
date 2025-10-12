namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetIntegralUserInfo
{
    public class GetIntegralUserInfoRequest
    {
        public long UserId { get; set; }
        public string MasterKey { get; set; } = string.Empty;
    }
}