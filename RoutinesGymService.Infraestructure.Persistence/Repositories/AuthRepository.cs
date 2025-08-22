using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
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
        private readonly IMemoryCache _cache;
        private readonly int _expiryMinutes;
        private readonly string _authPrefix;

        public AuthRepository(ApplicationDbContext context, IMemoryCache cache, IConfiguration configuration)
        {
            _cache = cache;
            _context = context;
            _authPrefix = configuration["CacheSettings:AuthPrefix"]!;
            _expiryMinutes = int.TryParse(configuration["CacheSettings:CacheExpiryMinutes"], out var m) ? m : 60;
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            LoginResponse loginResponse = new LoginResponse();
            try
            {
                string cacheKey = $"{_authPrefix}_Login_{loginRequest.UserEmail}";

                User? cacheUser = _cache.Get<User>(cacheKey);
                if (cacheUser != null)
                {
                    loginResponse.IsSuccess = true;
                    loginResponse.Message = "Login successful";
                    loginResponse.IsAdmin = cacheUser.RoleString.ToLower() == Role.ADMIN.ToString().ToLower();
                }
                else
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
                        loginResponse.IsSuccess = true;
                        loginResponse.Message = "Login successful.";
                        loginResponse.IsAdmin = user.RoleString.ToLower() == Role.ADMIN.ToString().ToLower();

                        _cache.Set(cacheKey, user, TimeSpan.FromMinutes(_expiryMinutes));
                    }
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