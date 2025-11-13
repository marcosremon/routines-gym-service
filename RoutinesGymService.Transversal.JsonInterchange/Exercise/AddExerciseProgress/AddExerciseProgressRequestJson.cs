namespace RoutinesGymService.Transversal.JsonInterchange.Exercise.AddExerciseProgress
{
    public class AddExerciseProgressRequestJson
    {
        public List<string> ProgressList { get; set; } = new List<string>();
        public string UserEmail { get; set; } = string.Empty;
        public long RoutineId { get; set; }
        public long splitDayId { get; set; }
        public string ExerciseName { get; set; } = string.Empty;
    }
}