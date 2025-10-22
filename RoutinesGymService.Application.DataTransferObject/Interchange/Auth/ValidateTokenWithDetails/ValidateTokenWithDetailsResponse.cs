using System.Security.Claims;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Auth.ValidateTokenWithDetails
{
    public class ValidateTokenWithDetailsResponse
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public ClaimsPrincipal Principal { get; set; } = new ClaimsPrincipal();
    }
}