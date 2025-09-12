namespace RoutinesGymService.Transversal.JsonInterchange.Step.GetDailyStepsInfo
{
    public class GetDailyStepsInfoRequestJson
    {
        public string? UserEmail { get; set; }
        public int? DailySteps { get; set; }
        public DateTime? Day { get; set; }
    }
}