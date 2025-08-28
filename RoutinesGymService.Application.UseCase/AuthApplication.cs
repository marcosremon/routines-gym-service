using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.CheckTokenStatus;
using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.Login;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Application.Interface.Repository;

namespace RoutinesGymService.Application.UseCase
{
    public class AuthApplication : IAuthApplication
    {
        private readonly IAuthRepository _authRepository;

        public AuthApplication(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public CheckTokenStatusResponse CheckTokenStatus(CheckTokenStatusRequest checkTokenStatusRequest)
        {
            return _authRepository.CheckTokenStatus(checkTokenStatusRequest);
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            return await _authRepository.Login(loginRequest);
        }
    }
}