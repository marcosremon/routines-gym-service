using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.ValidateTokenWithDetails;
using RoutinesGymService.Transversal.JsonInterchange.Auth.UnauthorizedObject;
using RoutinesGymService.Transversal.Security.SecurityUtils;

namespace RoutinesGymService.Transversal.Security.SecurityFilters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class JwtValidationFilter : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            HttpRequest request = context.HttpContext.Request;

            if (!request.Headers.TryGetValue("Authorization", out StringValues authHeader) ||
                string.IsNullOrWhiteSpace(authHeader) ||
                !authHeader.ToString().StartsWith("Bearer "))
            {
                SetUnauthorized(context, "Missing or invalid Authorization header");
                return;
            }

            string token = authHeader.ToString().Substring("Bearer ".Length).Trim();
            if (string.IsNullOrWhiteSpace(token))
            {
                SetUnauthorized(context, "Empty JWT token");
                return;
            }

            ValidateTokenWithDetailsResponse validationResult = JwtUtils.ValidateTokenWithDetails(token);
            if (!validationResult.IsValid || validationResult.Principal == null)
            {
                string error = validationResult.ErrorMessage ?? "Invalid or expired JWT token";
                SetUnauthorized(context, error);
                return;
            }

            context.HttpContext.User = validationResult.Principal;

            await next();
        }

        private void SetUnauthorized(ActionExecutingContext context, string message)
        {
            context.HttpContext.Items["CustomAuthResponse"] = true;
            context.Result = new UnauthorizedObjectResponse(message);
        }
    }
}
