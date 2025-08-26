using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Auth.Login
{
    public class LoginResponse : BaseResponse
    {
        public string BearerToken { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } 
    }
}