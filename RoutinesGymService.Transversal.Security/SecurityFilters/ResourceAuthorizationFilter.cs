using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using RoutinesGymService.Transversal.JsonInterchange.Auth.UnauthorizedObject;
using System.Reflection;
using System.Security.Claims;

namespace RoutinesGymService.Transversal.Security.SecurityFilters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class ResourceAuthorizationFilter : Attribute, IAsyncActionFilter
    {
        private readonly string[] _emailParameterNames;

        public ResourceAuthorizationFilter(params string[] emailParameterNames)
        {
            _emailParameterNames = emailParameterNames?.Any() == true
                ? emailParameterNames
                : new[] { "Email", "UserEmail", "OriginalEmail", "email", "userEmail" };
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ClaimsPrincipal user = context.HttpContext.User;
            string? tokenEmail = user.FindFirst(ClaimTypes.Email)?.Value;
            bool isAdmin = user.FindFirst(ClaimTypes.Role)?.Value == "ADMIN";

            if (string.IsNullOrEmpty(tokenEmail))
            {
                SetUnauthorizedResult(context, "Invalid token: missing email claim");
                return;
            }

            // Si es admin → acceso permitido
            if (isAdmin)
            {
                await next();
                return;
            }

            // Si no es admin → verificar email
            string? requestEmail = FindEmailInRequest(context);
            if (string.IsNullOrEmpty(requestEmail) || tokenEmail != requestEmail)
            {
                SetUnauthorizedResult(context, "Unauthorized: email mismatch");
                return;
            }

            await next();
        }

        private string? FindEmailInRequest(ActionExecutingContext context)
        {
            foreach (string paramName in _emailParameterNames)
            {
                string? emailFromArgs = GetEmailFromActionArguments(context, paramName);
                if (!string.IsNullOrEmpty(emailFromArgs))
                    return emailFromArgs;

                if (context.HttpContext.Request.HasFormContentType &&
                    context.HttpContext.Request.Form.TryGetValue(paramName, out StringValues formValue))
                    return formValue.ToString();

                if (context.HttpContext.Request.Query.TryGetValue(paramName, out StringValues queryValue))
                    return queryValue.ToString();

                if (context.RouteData.Values.TryGetValue(paramName, out object routeValue))
                    return routeValue?.ToString();
            }

            return null;
        }

        private string? GetEmailFromActionArguments(ActionExecutingContext context, string paramName)
        {
            try
            {
                if (context.ActionArguments.TryGetValue(paramName, out object? value))
                {
                    if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
                        return stringValue;
                }

                foreach (object argument in context.ActionArguments.Values)
                {
                    if (argument != null)
                    {
                        string? emailFromObject = GetEmailFromObject(argument, paramName);
                        if (!string.IsNullOrEmpty(emailFromObject))
                            return emailFromObject;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error buscando email en action arguments: {ex.Message}");
            }

            return null;
        }

        private string? GetEmailFromObject(object obj, string propertyName)
        {
            try
            {
                var property = obj.GetType().GetProperty(propertyName,
                    BindingFlags.IgnoreCase |
                    BindingFlags.Public |
                    BindingFlags.Instance);

                if (property != null)
                {
                    object? value = property.GetValue(obj);
                    if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
                        return stringValue;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo email del objeto: {ex.Message}");
            }

            return null;
        }

        private void SetUnauthorizedResult(ActionExecutingContext context, string message)
        {
            context.HttpContext.Items["CustomAuthResponse"] = true;
            context.Result = new UnauthorizedObjectResponse(message);
        }
    }
}