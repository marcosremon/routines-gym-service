namespace RoutinesGymService.Application.DataTransferObject.Entity
{
    public class ExerciseDTO
    {
        public string ExerciseName { get; set; } = string.Empty;
        public long RoutineId { get; set; }
        public long SplitDayId { get; set; }
    }
}