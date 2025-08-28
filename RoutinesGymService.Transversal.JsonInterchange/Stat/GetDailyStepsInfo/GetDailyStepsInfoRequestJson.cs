namespace RoutinesGymService.Transversal.JsonInterchange.Stat.GetDailyStepsInfo
{
    public class GetDailyStepsInfoRequestJson
    {
        public string UserEmail { get; set; } = string.Empty;
        public int? DailySteps { get; set; }
        public DateTime? Day { get; set; }
    }
}