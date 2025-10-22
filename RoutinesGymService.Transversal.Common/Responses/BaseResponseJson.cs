namespace RoutinesGymService.Transversal.Common.Responses
{
    public class BaseResponseJson
    {
        public ResponseCodesJson ResponseCodeJson { get; set; }
        public bool IsSuccess { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }
}