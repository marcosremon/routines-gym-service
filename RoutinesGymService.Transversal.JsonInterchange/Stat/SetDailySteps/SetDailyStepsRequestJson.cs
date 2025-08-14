namespace RoutinesGymService.Transversal.JsonInterchange.Stat.SetDailySteps
{
    public class SetDailyStepsRequestJson
    {
        public int Steps { get; set; }
        public int LimitStepsPerDay { get; set; }
        public DateTime Date { get; set; }
    }
}