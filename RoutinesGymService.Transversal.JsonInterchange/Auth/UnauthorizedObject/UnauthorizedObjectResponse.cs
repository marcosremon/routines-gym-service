using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Auth.UnauthorizedObject
{
    public class UnauthorizedObjectResponse : ObjectResult
    {
        public UnauthorizedObjectResponse(string message = "Unauthorized request")
            : base(new BaseResponseJson
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