namespace RoutinesGymService.Transversal.JsonInterchange.SplitDay.UpdateSplitDay
{
    public class UpdateSplitDayRequestJson
    {
        public long? RoutineId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public List<string> AddDays { get; set; } = new List<string>();
        public List<string> DeleteDays { get; set; } = new List<string>();
    }
}