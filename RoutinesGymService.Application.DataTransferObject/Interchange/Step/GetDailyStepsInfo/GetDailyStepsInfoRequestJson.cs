namespace RoutinesGymService.Application.DataTransferObject.Interchange.Step.GetDailyStepsInfo
{
    public class GetDailyStepsInfoRequest
    {
        public string UserEmail { get; set; } = string.Empty;
        public int? DailySteps { get; set; }
        public DateTime? Day { get; set; }
    }
}