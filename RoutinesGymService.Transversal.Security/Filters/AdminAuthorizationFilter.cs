using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.ValidateTokenWithDetails;
using RoutinesGymService.Transversal.JsonInterchange.Auth.UnauthorizedObject;
using RoutinesGymService.Transversal.Security.Utils;
using System.Security.Claims;

namespace RoutinesGymService.Transversal.Security.Filters
{
    public class AdminAuthorizationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                HttpContext httpContext = context.HttpContext;
                string? authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();

                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    context.Result = UnauthorizedObjectResponse.Unauthorized("Authorization header missing or malformed");
                    return;
                }

                string token = authHeader.Substring("Bearer ".Length).Trim();
                ValidateTokenWithDetailsResponse validationResult = JwtUtils.ValidateTokenWithDetails(token);

                if (!validationResult.IsValid || validationResult.Principal == null)
                {
                    context.Result = UnauthorizedObjectResponse.Unauthorized(validationResult.ErrorMessage ?? "Invalid or expired token");
                    return;
                }

                string? role = validationResult.Principal.FindFirst(ClaimTypes.Role)?.Value;
                if (string.IsNullOrWhiteSpace(role) || !role.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
                {
                    context.Result = UnauthorizedObjectResponse.Forbidden("Access denied. Admin privileges required.");
                    return;
                }

                await next();
            }
            catch (Exception ex)
            {
                context.Result = UnauthorizedObjectResponse.InternalServerError($"Unexpected error in AdminAuthorizationFilter: {ex.Message}");
            }
        }
    }
}