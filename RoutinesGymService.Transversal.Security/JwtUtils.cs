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
            string jwtKey = _configuration!["JWT:Key"]!;
            string issuer = _configuration["JWT:Issuer"]!;
            string audience = _configuration["JWT:Audience"]!;
            string expInMinutes = _configuration["JWT:ExpInMinutes"]!;

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(expInMinutes)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateUserJwtToken(string email)
        {
            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, Role.USER.ToString())
            };

            return GenerateJwtToken(claims);
        }

        public static string GenerateAdminJwtToken(string email)
        {
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, Role.ADMIN.ToString())
            };

            return GenerateJwtToken(claims);
        }

        public static bool IsValidToken(CheckTokenStatusRequest checkTokenStatusRequest)
        {
            if (_configuration == null) throw new InvalidOperationException("JwtUtils not initialized.");

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.UTF8.GetBytes(_configuration["jwt:Key"]!);

            bool isValid = false;
            try
            {
                tokenHandler.ValidateToken(checkTokenStatusRequest.Token, new TokenValidationParameters
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

                isValid = true;
            }
            catch
            {
                isValid = false;
            }

            return isValid;
        }
    }
}