namespace RoutinesGymService.Transversal.JsonInterchange.Exercise.DeleteExercise
{
    public class DeleteExerciseRequestJson
    {
        public string UserEmail { get; set; } = string.Empty;
        public int? RoutineId { get; set; }
        public string? DayName { get; set; }
        public int ExerciseId { get; set; }
    }
}