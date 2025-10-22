namespace RoutinesGymService.Transversal.JsonInterchange.Step.SaveDailySteps
{
    public class SaveDailyStepsRequestJson
    {
        public int Steps { get; set; } = -1;
        public string UserEmail { get; set; } = string.Empty;
        public int DailyStepsGoal { get; set; } = -1;
    }
}