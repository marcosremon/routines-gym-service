namespace RoutinesGymService.Application.DataTransferObject.Entity
{
    public class ExerciseDTO
    {
        public string ExerciseName { get; set; } = string.Empty;
        public long RoutineId { get; set; } = -1;
        public long SplitDayId { get; set; } = -1;
    }
}