namespace RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.DeleteExercise
{
    public class DeleteExerciseRequest
    {
        public string UserEmail { get; set; } = string.Empty;
        public long? RoutineId { get; set; }
        public string? DayName { get; set; }
        public long? ExerciseId { get; set; }
    }
}