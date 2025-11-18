using RoutinesGymService.Transversal.Common.Responses;

public class UnauthorizedObjectResponse
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