using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.ValidateTokenWithDetails;
using RoutinesGymService.Transversal.Security.Utils;
using System.Text.Json;

namespace RoutinesGymService.Transversal.Security.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class JwtValidationFilter : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpRequest = context.HttpContext.Request;

            if (!httpRequest.Headers.TryGetValue("Authorization", out StringValues authHeader) ||
                !authHeader.ToString().StartsWith("Bearer ") ||
                string.IsNullOrWhiteSpace(authHeader))
            {
                await _SetUnauthorizedAsync(context, "Missing or invalid Authorization header");
                return;
            }

            string token = authHeader.ToString().Substring("Bearer ".Length).Trim();

            if (string.IsNullOrWhiteSpace(token))
            {
                await _SetUnauthorizedAsync(context, "Empty JWT token");
                return;
            }

            ValidateTokenWithDetailsResponse validationResult = JwtUtils.ValidateTokenWithDetails(token);

            if (!validationResult.IsValid || validationResult.Principal == null)
            {
                string error = validationResult.ErrorMessage ?? "Invalid or expired JWT token";
                await _SetUnauthorizedAsync(context, error);
                return;
            }

            context.HttpContext.User = validationResult.Principal;
            await next();
        }

        private static async Task _SetUnauthorizedAsync(ActionExecutingContext context, string message)
        {
            var unauthorizedObjectResponse = UnauthorizedObjectResponse.Unauthorized(message);
            var httpContext = context.HttpContext;

            httpContext.Response.StatusCode = unauthorizedObjectResponse.StatusCode;
            httpContext.Response.ContentType = "application/json";

            string jsonResponse = JsonSerializer.Serialize(unauthorizedObjectResponse.Value);
            await httpContext.Response.WriteAsync(jsonResponse);
        }
    }
}