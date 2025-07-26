using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.Login;

namespace RoutinesGymService.Application.Interface.Application
{
    public interface IAuthApplication
    {
        Task<LoginResponse> Login(LoginRequest loginRequest);
    }
}