using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.Security
{
    public class UnauthorizedObjectResponse : ObjectResult
    {
        public UnauthorizedObjectResponse(BaseResponseJson response = null) : base(response ?? new BaseResponseJson
        {
            ResponseCodeJson = ResponseCodesJson.UNAUTHORIZED,
            IsSuccess = false,
            Message = "UNAUTHORIZED"
        })
        {
            StatusCode = 401;
        }

        public UnauthorizedObjectResponse(string message) : base(new BaseResponseJson
        {
            ResponseCodeJson = ResponseCodesJson.UNAUTHORIZED,
            IsSuccess = false,
            Message = message
        })
        {
            StatusCode = 401;
        }
    }
}