using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.ValidateTokenWithDetails;
using RoutinesGymService.Transversal.Security.Utils;
using System.Security.Claims;
using System.Text.Json;

namespace RoutinesGymService.Transversal.Security.Filters
{
    public class AdminAuthorizationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var httpContext = context.HttpContext;
                string? authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();

                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    await _WriteResponseAsync(httpContext, UnauthorizedObjectResponse.Unauthorized("Authorization header missing or malformed"));
                    return;
                }

                string token = authHeader.Substring("Bearer ".Length).Trim();
                ValidateTokenWithDetailsResponse validationResult = JwtUtils.ValidateTokenWithDetails(token);

                if (!validationResult.IsValid || validationResult.Principal == null)
                {
                    await _WriteResponseAsync(httpContext, UnauthorizedObjectResponse.Unauthorized(validationResult.ErrorMessage ?? "Invalid or expired token"));
                    return;
                }

                string? role = validationResult.Principal.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrWhiteSpace(role) || !role.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
                {
                    await _WriteResponseAsync(httpContext, UnauthorizedObjectResponse.Forbidden("Access denied. Admin privileges required."));
                    return;
                }

                await next();
            }
            catch (Exception ex)
            {
                await _WriteResponseAsync(context.HttpContext, UnauthorizedObjectResponse.InternalServerError($"Unexpected error in AdminAuthorizationFilter: {ex.Message}"));
            }
        }

        private static async Task _WriteResponseAsync(HttpContext httpContext, UnauthorizedObjectResponse unauthorizedObjectResponse)
        {
            httpContext.Response.StatusCode = unauthorizedObjectResponse.StatusCode;
            httpContext.Response.ContentType = "application/json";

            string jsonResponse = JsonSerializer.Serialize(unauthorizedObjectResponse.Value);
            await httpContext.Response.WriteAsync(jsonResponse);
        }
    }
}