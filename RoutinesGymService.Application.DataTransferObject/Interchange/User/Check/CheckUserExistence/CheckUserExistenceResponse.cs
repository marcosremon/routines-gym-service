using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.Check.CheckUserExistence
{
    public class CheckUserExistenceResponse : BaseResponse
    {
        public bool UserExists { get; set; }
    }
}