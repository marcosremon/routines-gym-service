using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using RoutinesGymService.Transversal.JsonInterchange.Auth;
using System.Reflection;
using System.Security.Claims;

namespace RoutinesGymService.Transversal.Security
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class ResourceAuthorizationAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string[] _emailParameterNames;

        public ResourceAuthorizationAttribute(params string[] emailParameterNames)
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
                SetUnauthorizedResult(context);
                return;
            }

            if (isAdmin)
            {
                await next();
                return;
            }

            // Si NO es admin → verificar que el email del token coincida con el del request
            string? requestEmail = FindEmailInRequest(context);
            if (string.IsNullOrEmpty(requestEmail) || tokenEmail != requestEmail)
            {
                SetUnauthorizedResult(context);
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
                {
                    return emailFromArgs;
                }

                if (context.HttpContext.Request.HasFormContentType &&
                    context.HttpContext.Request.Form.TryGetValue(paramName, out StringValues formValue))
                {
                    return formValue.ToString();
                }

                if (context.HttpContext.Request.Query.TryGetValue(paramName, out StringValues queryValue))
                {
                    return queryValue.ToString();
                }

                if (context.RouteData.Values.TryGetValue(paramName, out object routeValue))
                {
                    return routeValue?.ToString();
                }
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
                    {
                        return stringValue;
                    }
                }

                foreach (object argument in context.ActionArguments.Values)
                {
                    if (argument != null)
                    {
                        string? emailFromObject = GetEmailFromObject(argument, paramName);
                        if (!string.IsNullOrEmpty(emailFromObject))
                        {
                            return emailFromObject;
                        }
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
                Type type = obj.GetType();

                PropertyInfo? property = type.GetProperty(propertyName);
                if (property != null)
                {
                    object? value = property.GetValue(obj);
                    if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
                    {
                        return stringValue;
                    }
                }

                property = type.GetProperty(propertyName,
                    BindingFlags.IgnoreCase |
                    BindingFlags.Public |
                    BindingFlags.Instance);

                if (property != null)
                {
                    object? value = property.GetValue(obj);
                    if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
                    {
                        return stringValue;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo email del objeto: {ex.Message}");
            }

            return null;
        }

        private void SetUnauthorizedResult(ActionExecutingContext context)
        {
            context.HttpContext.Items["CustomAuthResponse"] = true;
            context.Result = new UnauthorizedObjectResponse();
        }
    }
}