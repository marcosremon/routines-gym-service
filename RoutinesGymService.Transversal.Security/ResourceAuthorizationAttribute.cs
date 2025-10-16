using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using System.Security.Claims;

namespace RoutinesGymService.Transversal.Security
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class ResourceAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _emailParameterNames;

        public ResourceAuthorizationAttribute(params string[] emailParameterNames)
        {
            // Si no se especifican parámetros, usar los comunes por defecto
            _emailParameterNames = emailParameterNames?.Any() == true
                ? emailParameterNames
                : new[] { "Email", "UserEmail", "OriginalEmail", "email", "userEmail" };
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            string? tokenEmail = user.FindFirst(ClaimTypes.Email)?.Value;
            bool isAdmin = user.FindFirst(ClaimTypes.Role)?.Value == "ADMIN";

            if (string.IsNullOrEmpty(tokenEmail))
            {
                SetUnauthorizedResult(context);
                return;
            }

            if (isAdmin)
            {
                return;
            }

            if (!CanAccessResource(context, tokenEmail))
            {
                SetUnauthorizedResult(context);
            }
        }

        private bool CanAccessResource(AuthorizationFilterContext context, string tokenEmail)
        {
            foreach (string paramName in _emailParameterNames)
            {
                string requestEmail = FindEmailInRequest(context, paramName);
                if (!string.IsNullOrEmpty(requestEmail) && tokenEmail == requestEmail)
                {
                    return true;
                }
            }

            return false;
        }

        private string? FindEmailInRequest(AuthorizationFilterContext context, string paramName)
        {
            string emailFromArgs = GetEmailFromActionArguments(context, paramName);
            if (!string.IsNullOrEmpty(emailFromArgs))
            {
                return emailFromArgs;
            }

            if (context.HttpContext.Request.HasFormContentType &&
                context.HttpContext.Request.Form.TryGetValue(paramName, out var formValue))
            {
                return formValue.ToString();
            }

            if (context.HttpContext.Request.Query.TryGetValue(paramName, out var queryValue))
            {
                return queryValue.ToString();
            }

            if (context.RouteData.Values.TryGetValue(paramName, out var routeValue))
            {
                return routeValue?.ToString();
            }

            return null;
        }

        private string? GetEmailFromActionArguments(AuthorizationFilterContext context, string paramName)
        {
            try
            {
                if (context.HttpContext.Items is IDictionary<string, object> actionArguments)
                {
                    foreach (object argument in actionArguments.Values)
                    {
                        if (argument == null) continue;

                        Type type = argument.GetType();

                        PropertyInfo? property = type.GetProperty(paramName);
                        if (property != null)
                        {
                            object? value = property.GetValue(argument);
                            if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
                            {
                                return stringValue;
                            }
                        }

                        property = type.GetProperty(paramName,
                            BindingFlags.IgnoreCase |
                            BindingFlags.Public |
                            BindingFlags.Instance);

                        if (property != null)
                        {
                            object? value = property.GetValue(argument);
                            if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
                            {
                                return stringValue;
                            }
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

        private void SetUnauthorizedResult(AuthorizationFilterContext context)
        {
            context.Result = new UnauthorizedObjectResponse();
        }
    }
}