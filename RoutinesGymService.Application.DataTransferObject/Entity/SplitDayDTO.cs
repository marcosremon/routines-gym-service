namespace RoutinesGymService.Application.DataTransferObject.Entity
{
    public class SplitDayDTO
    {
        public string DayName { get; set; } = string.Empty;
        public int RoutineId { get; set; }
        public string DayExercisesDescription { get; set; } = string.Empty;
    }
}