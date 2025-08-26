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
        private readonly PasswordUtils _passwordUtils;

        public AuthRepository(ApplicationDbContext context, PasswordUtils passwordUtils)
        {
            _context = context;
            _passwordUtils = passwordUtils;
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            LoginResponse loginResponse = new LoginResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u =>
                        u.Email == loginRequest.UserEmail &&
                        u.Password == _passwordUtils.PasswordEncoder(loginRequest.UserPassword));
                if (user == null)
                {
                    loginResponse.IsSuccess = false;
                    loginResponse.Message = "User not found";
                }
                else
                {
                    loginResponse.IsSuccess = true;
                    loginResponse.Message = "Login successful.";
                    loginResponse.IsAdmin = user.RoleString.ToLower() == Role.ADMIN.ToString().ToLower();
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