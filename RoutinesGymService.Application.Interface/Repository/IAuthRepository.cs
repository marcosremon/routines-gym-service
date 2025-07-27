using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.Login;

namespace RoutinesGymService.Application.Interface.Repository
{
    public interface IAuthRepository
    {
        Task<LoginResponse> Login(LoginRequest loginRequest);
    }
}