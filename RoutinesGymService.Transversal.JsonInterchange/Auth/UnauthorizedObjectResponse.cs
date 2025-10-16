using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Auth
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
            ContentTypes.Clear();
            ContentTypes.Add("application/json");
        }

        public UnauthorizedObjectResponse(string message) : base(new BaseResponseJson
        {
            ResponseCodeJson = ResponseCodesJson.UNAUTHORIZED,
            IsSuccess = false,
            Message = message
        })
        {
            StatusCode = 401;
            ContentTypes.Clear();
            ContentTypes.Add("application/json");
        }
    }
}