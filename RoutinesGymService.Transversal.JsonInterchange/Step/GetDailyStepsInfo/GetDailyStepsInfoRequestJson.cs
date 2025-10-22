namespace RoutinesGymService.Transversal.JsonInterchange.Step.GetDailyStepsInfo
{
    public class GetDailyStepsInfoRequestJson
    {
        public string UserEmail { get; set; } = string.Empty;
        public int DailySteps { get; set; } = -1;
        public DateTime Day { get; set; } = DateTime.MinValue;
    }
}