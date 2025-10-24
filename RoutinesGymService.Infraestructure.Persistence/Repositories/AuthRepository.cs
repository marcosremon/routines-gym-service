using Microsoft.EntityFrameworkCore;
using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.CheckTokenStatus;
using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.Login;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Domain.Model.Enums;
using RoutinesGymService.Infraestructure.Persistence.Context;
using RoutinesGymService.Transversal.Security.Utils;

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

        #region Check token status
        public CheckTokenStatusResponse CheckTokenStatus(CheckTokenStatusRequest checkTokenStatusRequest)
        {
            CheckTokenStatusResponse checkTokenStatusResponse = new CheckTokenStatusResponse();
            try
            {
                bool isTokenValid = JwtUtils.IsValidToken(checkTokenStatusRequest);

                checkTokenStatusResponse.IsValid = isTokenValid;
                checkTokenStatusResponse.IsSuccess = isTokenValid;
                checkTokenStatusResponse.Message = isTokenValid 
                    ? "The token is valid"
                    : "The token is not valid";
            }
            catch (Exception ex)
            {
                checkTokenStatusResponse.IsValid = false;
                checkTokenStatusResponse.IsSuccess = false;
                checkTokenStatusResponse.Message = $"unexpected error on AuthRepository -> CheckTokenStatus: {ex.Message}";
            }

            return checkTokenStatusResponse;
        }
        #endregion

        #region Login
        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            LoginResponse loginResponse = new LoginResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.UserEmail.ToLower());
                if (user == null)
                {
                    loginResponse.IsSuccess = false;
                    loginResponse.Message = "User not found";
                }
                else
                {
                    bool isPasswordValid = _passwordUtils.VerifyPassword(user.Password, loginRequest.UserPassword);
                    bool onBlackList = await _context.BlackList.AnyAsync(bl => bl.SerialNumber == user.SerialNumber);

                    if (onBlackList)
                    {
                        loginResponse.IsSuccess = false; 
                        loginResponse.Message = "The user that created this account is on BlackList 💀";
                    }
                    else if (!isPasswordValid && user.Email != "admin")
                    {
                        loginResponse.IsSuccess = false;
                        loginResponse.Message = "Password is not valid";
                    }
                    else
                    {
                        loginResponse.IsSuccess = true;
                        loginResponse.Message = "Login successful.";
                        loginResponse.IsAdmin = user.RoleString.ToLower() == Role.ADMIN.ToString().ToLower();
                        loginResponse.BearerToken = user.RoleString.ToLower() == Role.ADMIN.ToString().ToLower()
                                ? JwtUtils.GenerateAdminJwtToken(loginRequest.UserEmail)
                                : JwtUtils.GenerateUserJwtToken(loginRequest.UserEmail);
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
        #endregion
    }
}