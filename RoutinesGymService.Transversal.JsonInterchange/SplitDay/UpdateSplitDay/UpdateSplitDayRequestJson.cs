namespace RoutinesGymService.Transversal.JsonInterchange.SplitDay.UpdateSplitDay
{
    public class UpdateSplitDayRequestJson
    {
        public string RoutineName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public List<string> AddDays { get; set; } = new List<string>();
        public List<string> DeleteDays { get; set; } = new List<string>();
    }
}