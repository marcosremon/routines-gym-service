namespace RoutinesGymService.Application.DataTransferObject.SplitDay.UpdateSplitDay
{
    public class UpdateSplitDayRequest
    {
        public long? RoutineId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public List<string> AddDays { get; set; } = new List<string>();
        public List<string> DeleteDays { get; set; } = new List<string>();
    }
}