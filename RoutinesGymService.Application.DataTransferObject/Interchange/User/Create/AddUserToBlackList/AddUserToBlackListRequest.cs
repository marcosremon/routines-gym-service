namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.AddUserToBlackList
{
    public class AddUserToBlackListRequest
    {
        public string SerialNumber { get; set; } = string.Empty;
        public long UserId { get; set; }
    }
}