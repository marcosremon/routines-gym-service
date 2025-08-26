namespace RoutinesGymService.Transversal.Common.Responses
{
    public class BaseResponse
    {
        public ResponseCodes ResponsCode { get; set; }
        public bool IsSuccess { get; set; } = false;
        public string? Message { get; set; }
    }
}