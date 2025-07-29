namespace RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExercise
{
    public class AddExerciseRequest
    {
        public long? RoutineId { get; set; }
        public string ExerciseName { get; set; } = string.Empty;
        public string DayName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }
}