using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.DeleteUser
{
    public class DeleteUserResponse : BaseResponse
    {
        public long UserId { get; set; }
    }
}