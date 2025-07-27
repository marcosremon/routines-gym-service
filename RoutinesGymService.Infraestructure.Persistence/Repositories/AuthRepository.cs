using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.Login;
using RoutinesGymService.Application.Interface.Repository;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        public Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            throw new NotImplementedException();
        }
    }
}