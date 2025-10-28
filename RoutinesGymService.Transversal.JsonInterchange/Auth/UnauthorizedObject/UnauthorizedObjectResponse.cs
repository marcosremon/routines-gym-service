using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Transversal.Common.Responses;

public class UnauthorizedObjectResponse : ObjectResult
{
    private UnauthorizedObjectResponse(int statusCode, string message) 
        : base(new BaseResponseJson
        {
            ResponseCodeJson = GetResponseCodeFromStatusCode(statusCode),
            IsSuccess = false,
            Message = message
        })
    {
        StatusCode = statusCode;
    }

    public static UnauthorizedObjectResponse Unauthorized(string message = "Unauthorized request") 
        => new UnauthorizedObjectResponse(StatusCodes.Status401Unauthorized, message);

    public static UnauthorizedObjectResponse Forbidden(string message = "Access denied")
        => new UnauthorizedObjectResponse(StatusCodes.Status403Forbidden, message);

    public static UnauthorizedObjectResponse InternalServerError(string message = "Internal server error")
        => new UnauthorizedObjectResponse(StatusCodes.Status500InternalServerError, message);

    private static ResponseCodesJson GetResponseCodeFromStatusCode(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status401Unauthorized => ResponseCodesJson.UNAUTHORIZED,
            StatusCodes.Status403Forbidden => ResponseCodesJson.FORBIDDEN,
            StatusCodes.Status500InternalServerError => ResponseCodesJson.INTERNAL_SERVER_ERROR,
            _ => ResponseCodesJson.UNAUTHORIZED
        };
    }
}