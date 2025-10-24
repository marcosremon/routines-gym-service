using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.ValidateTokenWithDetails;
using RoutinesGymService.Transversal.Common.Responses;
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
                    context.Result = new UnauthorizedObjectResponse("Authorization header missing or malformed");
                    return;
                }

                string token = authHeader.Substring("Bearer ".Length).Trim();
                ValidateTokenWithDetailsResponse validationResult = JwtUtils.ValidateTokenWithDetails(token);

                if (!validationResult.IsValid || validationResult.Principal == null)
                {
                    context.Result = new UnauthorizedObjectResponse(validationResult.ErrorMessage ?? "Invalid or expired token");
                    return;
                }

                string? role = validationResult.Principal.FindFirst(ClaimTypes.Role)?.Value;
                if (string.IsNullOrWhiteSpace(role) || !role.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
                {
                    context.Result = new ObjectResult(new BaseResponseJson
                    {
                        ResponseCodeJson = ResponseCodesJson.FORBIDDEN,
                        IsSuccess = false,
                        Message = "Access denied. Admin privileges required."
                    })
                    {
                        StatusCode = 403
                    };
                    return;
                }

                await next();
            }
            catch (Exception ex)
            {
                context.Result = new ObjectResult(new BaseResponseJson
                {
                    ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR,
                    IsSuccess = false,
                    Message = $"Unexpected error in AdminAuthorizationFilter: {ex.Message}"
                })
                {
                    StatusCode = 500
                };
            }
        }
    }
}