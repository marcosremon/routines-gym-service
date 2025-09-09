namespace RoutinesGymService.Transversal.JsonInterchange.Exercise.GetAllExerciseProgress
{
    public class GetAllExerciseProgressRequestJson
    {
        public string UserEmail { get; set; } = string.Empty;
        public string ExerciseName { get; set; } = string.Empty;
        public string RoutineName { get; set; } = string.Empty;
        public string DayName { get; set; } = string.Empty;
    }
}