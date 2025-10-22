namespace RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetIntegralUserInfo
{
    public class GetIntegralUserInfoRequest
    {
        public long UserId { get; set; } = -1;
        public string MasterKey { get; set; } = string.Empty;
    }
}