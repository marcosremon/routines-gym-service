namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.AddUserToBlackList
{
    public class AddUserToBlackListRequest
    {
        public long UserId { get; set; } = -1;
        public string SerialNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}