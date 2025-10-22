using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.CheckTokenStatus;
using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.ValidateTokenWithDetails;
using RoutinesGymService.Domain.Model.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RoutinesGymService.Transversal.Security
{
    public class JwtUtils
    {
        private static IConfiguration? _configuration;
        private static ILogger? _logger;

        public static void Initialize(IConfiguration configuration, ILogger? logger = null)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;

            ValidateJwtConfiguration();
        }

        private static void ValidateJwtConfiguration()
        {
            if (_configuration == null) return;

            string? key = _configuration["JWT:Key"];
            string? issuer = _configuration["JWT:Issuer"];
            string? audience = _configuration["JWT:Audience"];

            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("JWT:Key is not configured in appsettings.json");

            if (key.Length < 32)
                throw new InvalidOperationException("JWT:Key must be at least 32 characters for HS256 security");

            if (string.IsNullOrWhiteSpace(issuer))
                throw new InvalidOperationException("JWT:Issuer is not configured in appsettings.json");

            if (string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("JWT:Audience is not configured in appsettings.json");

            _logger?.LogInformation("JWT configuration validated successfully");
        }

        public static string GenerateJwtToken(Claim[] claims)
        {
            if (_configuration == null)
                throw new InvalidOperationException("JwtUtils not initialized. Call Initialize() first.");

            if (claims == null || claims.Length == 0)
                throw new ArgumentException("Claims cannot be null or empty", nameof(claims));

            string jwtKey = _configuration["JWT:Key"]!;
            string issuer = _configuration["JWT:Issuer"]!;
            string audience = _configuration["JWT:Audience"]!;
            string expInMinutes = _configuration["JWT:ExpInMinutes"] ?? "60";

            if (!int.TryParse(expInMinutes, out int expirationMinutes) || expirationMinutes <= 0)
            {
                _logger?.LogWarning("Invalid JWT:ExpInMinutes value '{ExpInMinutes}'. Using default 60 minutes.", expInMinutes);
                expirationMinutes = 60;
            }

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: creds);

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            _logger?.LogDebug("JWT token generated for subject: {Subject}",
                claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value ?? "unknown");

            return tokenString;
        }

        public static string GenerateUserJwtToken(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));

            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, Role.USER.ToString())
            };

            _logger?.LogInformation("Generating USER token for email: {Email}", email);
            return GenerateJwtToken(claims);
        }

        public static string GenerateAdminJwtToken(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));

            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, Role.ADMIN.ToString())
            };

            _logger?.LogInformation("Generating ADMIN token for email: {Email}", email);
            return GenerateJwtToken(claims);
        }

        public static bool IsValidToken(CheckTokenStatusRequest checkTokenStatusRequest)
        {
            if (_configuration == null)
                throw new InvalidOperationException("JwtUtils not initialized. Call Initialize() first.");

            if (checkTokenStatusRequest == null || string.IsNullOrWhiteSpace(checkTokenStatusRequest.Token))
            {
                _logger?.LogWarning("Token validation failed: Token is null or empty");
                return false;
            }

            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                byte[] key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!);

                tokenHandler.ValidateToken(checkTokenStatusRequest.Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JWT:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                _logger?.LogDebug("Token validated successfully");
                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                _logger?.LogWarning("Token validation failed: Token expired");
                return false;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                _logger?.LogWarning("Token validation failed: Invalid signature");
                return false;
            }
            catch (SecurityTokenInvalidIssuerException)
            {
                _logger?.LogWarning("Token validation failed: Invalid issuer");
                return false;
            }
            catch (SecurityTokenInvalidAudienceException)
            {
                _logger?.LogWarning("Token validation failed: Invalid audience");
                return false;
            }
            catch (SecurityTokenException ex)
            {
                _logger?.LogWarning("Token validation failed: {Message}", ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unexpected error validating token");
                return false;
            }
        }

        public static ValidateTokenWithDetailsResponse ValidateTokenWithDetails(string token)
        {
            ValidateTokenWithDetailsResponse validateTokenWithDetailsResponse = new ValidateTokenWithDetailsResponse();
            if (_configuration == null)
                throw new InvalidOperationException("JwtUtils not initialized. Call Initialize() first.");

            if (string.IsNullOrWhiteSpace(token))
            {
                validateTokenWithDetailsResponse.IsValid = false;
                validateTokenWithDetailsResponse.ErrorMessage = "Token is null or empty";
                validateTokenWithDetailsResponse.Principal = null;
            }
            else
            {
                try
                {
                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                    byte[] key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!);

                    ClaimsPrincipal principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = _configuration["JWT:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = _configuration["JWT:Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    validateTokenWithDetailsResponse.IsValid = true;
                    validateTokenWithDetailsResponse.ErrorMessage = null;
                    validateTokenWithDetailsResponse.Principal = principal;
                }
                catch (SecurityTokenExpiredException)
                {
                    validateTokenWithDetailsResponse.IsValid = false;
                    validateTokenWithDetailsResponse.ErrorMessage = "Token expired";
                    validateTokenWithDetailsResponse.Principal = null;
                }
                catch (SecurityTokenInvalidSignatureException)
                {
                    validateTokenWithDetailsResponse.IsValid = false;
                    validateTokenWithDetailsResponse.ErrorMessage = "Invalid signature";
                    validateTokenWithDetailsResponse.Principal = null;
                }
                catch (SecurityTokenInvalidIssuerException)
                {
                    validateTokenWithDetailsResponse.IsValid = false;
                    validateTokenWithDetailsResponse.ErrorMessage = "Invalid issuer";
                    validateTokenWithDetailsResponse.Principal = null;
                }
                catch (SecurityTokenInvalidAudienceException)
                {
                    validateTokenWithDetailsResponse.IsValid = false;
                    validateTokenWithDetailsResponse.ErrorMessage = "Invalid audience";
                    validateTokenWithDetailsResponse.Principal = null;
                }
                catch (SecurityTokenException ex)
                {
                    validateTokenWithDetailsResponse.IsValid = false;
                    validateTokenWithDetailsResponse.ErrorMessage = ex.Message;
                    validateTokenWithDetailsResponse.Principal = null;
                }
                catch (Exception ex)
                {
                    validateTokenWithDetailsResponse.IsValid = false;
                    validateTokenWithDetailsResponse.ErrorMessage = "Unexpected error";
                    validateTokenWithDetailsResponse.Principal = null;
                    _logger?.LogError(ex, "Unexpected error validating token");
                }
            }

            return validateTokenWithDetailsResponse;
        }

        public static ClaimsPrincipal? GetClaimsWithoutValidation(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

                ClaimsIdentity identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
                return new ClaimsPrincipal(identity);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to read token claims without validation");
                return null;
            }
        }

        public static string? GetEmailFromToken(string token)
        {
            ClaimsPrincipal? principal = GetClaimsWithoutValidation(token);
            return principal?.FindFirst(ClaimTypes.Email)?.Value
                ?? principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        }

        public static string? GetRoleFromToken(string token)
        {
            ClaimsPrincipal? principal = GetClaimsWithoutValidation(token);
            return principal?.FindFirst(ClaimTypes.Role)?.Value;
        }

        public static bool IsTokenExpired(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return true;

            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

                return jwtToken.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }

        public static DateTime? GetTokenExpirationDate(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

                return jwtToken.ValidTo;
            }
            catch
            {
                return null;
            }
        }

        public static TimeSpan? GetTokenRemainingTime(string token)
        {
            DateTime? expirationDate = GetTokenExpirationDate(token);
            if (expirationDate == null)
                return null;

            TimeSpan remaining = expirationDate.Value - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
    }   
}