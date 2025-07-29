namespace RoutinesGymService.Application.DataTransferObject.Entity
{
    public class ExerciseDTO
    {
        public string ExerciseName { get; set; } = string.Empty;
        public int RoutineId { get; set; }
        public int SplitDayId { get; set; }
    }
}