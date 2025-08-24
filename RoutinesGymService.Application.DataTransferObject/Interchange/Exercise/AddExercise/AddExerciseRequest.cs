namespace RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExercise
{
    public class AddExerciseRequest
    {
        public string RoutineName { get; set; } = string.Empty;
        public string ExerciseName { get; set; } = string.Empty;
        public string DayName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }
}