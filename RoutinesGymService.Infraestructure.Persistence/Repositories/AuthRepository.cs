using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.BlackListValidation;
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
                User? user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == loginRequest.UserEmail.ToLower());
                
                BlackListValidationRequest blackListValidationRequest = new BlackListValidationRequest
                {
                    MobileGuid = loginRequest.MobileGuid ?? string.Empty,
                    UserSerialNumber = user?.SerialNumber ?? string.Empty,
                    EncryptedPassword = user?.Password ?? Array.Empty<byte>(),
                    UserPassword = loginRequest.UserPassword
                };

                BlackListValidationResponse blackListValidationResponse = await BlackListValidation(blackListValidationRequest);
                if (!blackListValidationResponse.IsSuccess)
                {
                    loginResponse.IsSuccess = false;
                    loginResponse.Message = blackListValidationResponse.Message;
                }
                else if (user == null) 
                {
                    loginResponse.IsSuccess = false;
                    loginResponse.Message = "Invalid credentials"; 
                }
                else
                {
                    loginResponse.IsSuccess = true;
                    loginResponse.Message = "Login successful.";
                    loginResponse.IsAdmin = user.RoleString.ToLower() == Role.ADMIN.ToString().ToLower();
                    loginResponse.BearerToken = JwtUtils.GenerateJwtWithRole(user.RoleString, loginRequest.UserEmail);
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

        #region Auxiliary methods

        #region Black list validation
        public async Task<BlackListValidationResponse> BlackListValidation(BlackListValidationRequest blackListValidationRequest)
        {
            BlackListValidationResponse blackListValidationResponse = new BlackListValidationResponse();
            try
            {
                if (string.IsNullOrEmpty(blackListValidationRequest.MobileGuid) ||
                    string.IsNullOrEmpty(blackListValidationRequest.UserSerialNumber) ||
                    string.IsNullOrEmpty(blackListValidationRequest.UserPassword) ||
                    blackListValidationRequest.EncryptedPassword == Array.Empty<byte>())
                {
                    blackListValidationResponse.IsSuccess = false;
                    blackListValidationResponse.Message = "Invalid data provided for blacklist validation";
                }
                else
                {
                    List<string> blackListSerials = await _context.BlackList
                                .Where(bl => bl.SerialNumber == blackListValidationRequest.MobileGuid ||
                                             bl.SerialNumber == blackListValidationRequest.UserSerialNumber)
                                .Select(bl => bl.SerialNumber)
                                .ToListAsync();

                    bool deviceOnBlackList = blackListSerials.Contains(blackListValidationRequest.MobileGuid);
                    bool userOnBlackList = blackListSerials.Contains(blackListValidationRequest.UserSerialNumber);
                    bool isPasswordValid = _passwordUtils.VerifyPassword(blackListValidationRequest.EncryptedPassword, blackListValidationRequest.UserPassword);

                    if (deviceOnBlackList)
                    {
                        blackListValidationResponse.Message = "The mobile device you are using is on the blacklist. Contact the app creator";
                        blackListValidationResponse.IsSuccess = false;
                    }
                    else if (userOnBlackList)
                    {
                        blackListValidationResponse.Message = "The user that created this account is on BlackList 💀";
                        blackListValidationResponse.IsSuccess = false;
                    }
                    else if (!isPasswordValid)
                    {
                        blackListValidationResponse.Message = "The password provided does not match the encrypted password";
                        blackListValidationResponse.IsSuccess = false;
                    }
                    else
                    {
                        blackListValidationResponse.IsSuccess = true;
                    }
                } 
            }
            catch 
            {
                blackListValidationResponse.IsSuccess = false;
                blackListValidationResponse.Message = $"unexpected error on AuthRepository -> BlackListValidation";
            }

            return blackListValidationResponse;
        }
        #endregion

        #endregion
    }
}