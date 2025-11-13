namespace RoutinesGymService.Application.DataTransferObject.Entity
{
    public class RoutineDTO
    {
        public string RoutineName { get; set; } = string.Empty;
        public string RoutineDescription { get; set; } = string.Empty;
        public long UserId { get; set; }
        public List<SplitDayDTO> SplitDays { get; set; } = new List<SplitDayDTO>();
    }
}