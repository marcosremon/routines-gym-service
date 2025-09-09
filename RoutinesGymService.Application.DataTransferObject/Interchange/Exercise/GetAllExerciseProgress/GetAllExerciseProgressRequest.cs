namespace RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.GetAllExerciseProgress
{
    public class GetAllExerciseProgressRequest
    {
        public string UserEmail { get; set; } = string.Empty;
        public string ExerciseName { get; set; } = string.Empty;
        public string RoutineName { get; set; } = string.Empty;
        public string DayName { get; set; } = string.Empty;
    }
}