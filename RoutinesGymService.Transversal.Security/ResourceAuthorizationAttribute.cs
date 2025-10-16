using Microsoft.AspNetCore.Mvc.Filters;
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
            var user = context.HttpContext.User;
            string? tokenEmail = user.FindFirst(ClaimTypes.Email)?.Value;
            bool isAdmin = user.FindFirst(ClaimTypes.Role)?.Value == "ADMIN";

            // Si el email del token es null o empty → UNAUTHORIZED
            if (string.IsNullOrEmpty(tokenEmail))
            {
                SetUnauthorizedResult(context);
                return;
            }

            // Si es admin → PERMITIR ACCESO (bypass)
            if (isAdmin)
            {
                await next();
                return;
            }

            // Si NO es admin → verificar que el email del token coincida con el del request
            string? requestEmail = FindEmailInRequest(context);

            // Si no encuentra email en el request O no coincide con el del token → UNAUTHORIZED
            if (string.IsNullOrEmpty(requestEmail) || tokenEmail != requestEmail)
            {
                SetUnauthorizedResult(context);
                return;
            }

            // Si llegó aquí: email del token == email del request → PERMITIR ACCESO
            await next();
        }

        private string? FindEmailInRequest(ActionExecutingContext context)
        {
            // Busca el email usando los nombres de parámetros configurados
            foreach (string paramName in _emailParameterNames)
            {
                // 1. Buscar en action arguments (AHORA SÍ FUNCIONA porque estamos en ActionExecutingContext)
                string? emailFromArgs = GetEmailFromActionArguments(context, paramName);
                if (!string.IsNullOrEmpty(emailFromArgs))
                {
                    return emailFromArgs;
                }

                // 2. Buscar en form data
                if (context.HttpContext.Request.HasFormContentType &&
                    context.HttpContext.Request.Form.TryGetValue(paramName, out var formValue))
                {
                    return formValue.ToString();
                }

                // 3. Buscar en query parameters
                if (context.HttpContext.Request.Query.TryGetValue(paramName, out var queryValue))
                {
                    return queryValue.ToString();
                }

                // 4. Buscar en route parameters
                if (context.RouteData.Values.TryGetValue(paramName, out var routeValue))
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
                // AHORA ActionArguments ESTÁ DISPONIBLE Y LLENO
                // Buscar directamente en los argumentos por nombre
                if (context.ActionArguments.TryGetValue(paramName, out var value))
                {
                    if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
                    {
                        return stringValue;
                    }
                }

                // Buscar en todos los objetos de los argumentos
                foreach (var argument in context.ActionArguments.Values)
                {
                    if (argument != null)
                    {
                        var emailFromObject = GetEmailFromObject(argument, paramName);
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

                // Buscar la propiedad con el nombre exacto
                PropertyInfo? property = type.GetProperty(propertyName);
                if (property != null)
                {
                    object? value = property.GetValue(obj);
                    if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
                    {
                        return stringValue;
                    }
                }

                // Buscar ignorando mayúsculas/minúsculas
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