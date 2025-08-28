using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.CheckTokenStatus;
using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.Login;

namespace RoutinesGymService.Application.Interface.Repository
{
    public interface IAuthRepository
    {
        CheckTokenStatusResponse CheckTokenStatus(CheckTokenStatusRequest checkTokenStatusRequest);
        Task<LoginResponse> Login(LoginRequest loginRequest);
    }
}