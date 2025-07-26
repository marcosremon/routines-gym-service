namespace RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExercise
{
    public class AddExerciseRequest
    {
        public int RoutineId { get; set; }
        public string ExerciseName { get; set; } = string.Empty;
        public string DayName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public int Sets { get; set; }
        public int Reps { get; set; }
        public double Weight { get; set; }
    }
}