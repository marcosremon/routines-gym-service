namespace RoutinesGymService.Transversal.JsonInterchange.Stat.SetDailySteps
{
    public class SetDailyStepsRequest
    {
        public int Steps { get; set; }
        public int LimitStepsPerDay { get; set; }
        public DateTime Date { get; set; }
    }
}