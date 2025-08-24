namespace RoutinesGymService.Transversal.JsonInterchange.Exercise.DeleteExercise
{
    public class DeleteExerciseRequestJson
    {
        public string UserEmail { get; set; } = string.Empty;
        public long? RoutineId { get; set; }
        public string DayName { get; set; } = string.Empty;
        public string ExerciseName { get; set; } = string.Empty;
    }
}