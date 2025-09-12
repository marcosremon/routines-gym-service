namespace RoutinesGymService.Transversal.JsonInterchange.Step.SaveDailySteps
{
    public class SaveDailyStepsRequestJson
    {
        public int? Steps { get; set; }
        public string? UserEmail { get; set; }
        public int? DailyStepsGoal { get; set; }
    }
}