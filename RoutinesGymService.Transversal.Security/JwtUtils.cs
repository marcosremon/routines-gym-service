using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.CheckTokenStatus;
using RoutinesGymService.Domain.Model.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RoutinesGymService.Transversal.Security
{
    public class JwtUtils
    {
        private static IConfiguration? _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string GenerateJwtToken(Claim[] claims)
        {
            var jwtKey = _configuration["JWT:Key"];
            var issuer = _configuration["JWT:Issuer"];
            var audience = _configuration["JWT:Audience"];
            var expInMinutes = _configuration["JWT:ExpInMinutes"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(expInMinutes)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateUserJwtToken(string username)
        {
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, Role.USER.ToString())
            };

            return GenerateJwtToken(claims);
        }

        public static string GenerateAdminJwtToken(string username)
        {
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("role", Role.ADMIN.ToString())
            };

            return GenerateJwtToken(claims);
        }

        public static CheckTokenStatusResponse IsValidToken(string token)
        {
            if (_configuration == null) throw new InvalidOperationException("JwtUtils not initialized.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["jwt:Key"]);

            CheckTokenStatusResponse checkTokenStatusResponse = new CheckTokenStatusResponse();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                checkTokenStatusResponse.IsValid = true;
            }
            catch
            {
                checkTokenStatusResponse.IsValid = false;
            }

            return checkTokenStatusResponse;
        }
    }
}