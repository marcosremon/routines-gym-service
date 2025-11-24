using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Http;
using RoutinesGymService.Transversal.Common.Responses;
using System.Text.Json;

namespace RoutinesGymService.Transversal.JsonInterchange.Auth.UnauthorizedObject
{
    public class UnauthorizedObjectResponse : IActionResult
    {
        private const int Status401Unauthorized = 401;
        private const int Status403Forbidden = 403;
        private const int Status500InternalServerError = 500;

        public BaseResponseJson Value { get; }
        public int StatusCode { get; }

        private UnauthorizedObjectResponse(int statusCode, string message)
        {
            Value = new BaseResponseJson
            {
                ResponseCodeJson = _GetResponseCodeFromStatusCode(statusCode),
                IsSuccess = false,
                Message = message
            };
            StatusCode = statusCode;
        }

        // Método requerido por IActionResult
        public async Task ExecuteResultAsync(ActionContext context)
        {
            // 1. Configurar el código de estado (StatusCode)
            context.HttpContext.Response.StatusCode = this.StatusCode;

            // 2. Serializar el objeto 'Value' a JSON
            var json = JsonSerializer.Serialize(this.Value);

            // 3. Escribir el JSON en el cuerpo de la respuesta
            context.HttpContext.Response.ContentType = "application/json";
            await context.HttpContext.Response.WriteAsync(json);
        }

        public static UnauthorizedObjectResponse Unauthorized(string message = "Unauthorized request")
            => new UnauthorizedObjectResponse(Status401Unauthorized, message);

        public static UnauthorizedObjectResponse Forbidden(string message = "Access denied")
            => new UnauthorizedObjectResponse(Status403Forbidden, message);

        public static UnauthorizedObjectResponse InternalServerError(string message = "Internal server error")
            => new UnauthorizedObjectResponse(Status500InternalServerError, message);

        private static ResponseCodesJson _GetResponseCodeFromStatusCode(int statusCode)
        {
            return statusCode switch
            {
                Status401Unauthorized => ResponseCodesJson.UNAUTHORIZED,
                Status403Forbidden => ResponseCodesJson.FORBIDDEN,
                Status500InternalServerError => ResponseCodesJson.INTERNAL_SERVER_ERROR,
                _ => ResponseCodesJson.UNAUTHORIZED
            };
        }
    }
}