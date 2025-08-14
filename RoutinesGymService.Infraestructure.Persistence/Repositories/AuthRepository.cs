using Microsoft.EntityFrameworkCore;
using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.Login;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Domain.Model.Enums;
using RoutinesGymService.Infraestructure.Persistence.Context;
using RoutinesGymService.Transversal.Security;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            LoginResponse loginResponse = new LoginResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u =>
                    u.Email == loginRequest.UserEmail &&
                    u.Password == PasswordUtils.PasswordEncoder(loginRequest.UserPassword));
                if (user == null)
                {
                    loginResponse.IsSuccess = false;
                    loginResponse.Message = "User not found";
                }
                else
                {
                    loginResponse.IsAdmin = user.RoleString.ToLower() == Role.ADMIN.ToString().ToLower();
                    loginResponse.IsSuccess = true;
                    loginResponse.Message = "Login successful.";
                }
            }
            catch (Exception ex)
            {
                loginResponse.IsSuccess = false;
                loginResponse.Message = $"unexpected error on AuthRepository -> login: {ex.Message}";
            }

            return loginResponse;
        }
    }
}