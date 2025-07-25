using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateNewPassword
{
    public class CreateNewPasswordResponse : BaseResponse
    {
        public long UserId { get; set; }
    }
}