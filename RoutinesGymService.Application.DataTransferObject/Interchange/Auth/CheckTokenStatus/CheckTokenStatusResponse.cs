using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Auth.CheckTokenStatus
{
    public class CheckTokenStatusResponse : BaseResponse
    {
        public bool IsValid { get; set; } = false;
    }
}