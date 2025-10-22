using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.Check.UserExist
{
    public class UserExistResponse : BaseResponse
    {
        public bool DniExists { get; set; }
        public bool EmailExists { get; set; }
        public bool UserExist { get; set; }
    }
}