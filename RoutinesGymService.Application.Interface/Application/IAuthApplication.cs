using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.CheckTokenStatus;
using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.Login;

namespace RoutinesGymService.Application.Interface.Application
{
    public interface IAuthApplication
    {
        CheckTokenStatusResponse CheckTokenStatus(CheckTokenStatusRequest checkTokenStatusRequest);
        Task<LoginResponse> Login(LoginRequest loginRequest);
    }
}