namespace RoutinesGymService.Transversal.JsonInterchange.Stat.SaveDailySteps
{
    public class SaveDailyStepsRequestJson
    {
        public int? Steps { get; set; }
        public string? UserEmail { get; set; }
        public int? DailyStepsGoal { get; set; }
    }
}