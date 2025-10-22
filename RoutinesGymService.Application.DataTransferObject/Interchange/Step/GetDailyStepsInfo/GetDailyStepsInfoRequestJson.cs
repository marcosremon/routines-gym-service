namespace RoutinesGymService.Application.DataTransferObject.Interchange.Step.GetDailyStepsInfo
{
    public class GetDailyStepsInfoRequest
    {
        public string UserEmail { get; set; } = string.Empty;
        public int DailySteps { get; set; } = -1;
        public DateTime Day { get; set; } = DateTime.MinValue;
    }
}