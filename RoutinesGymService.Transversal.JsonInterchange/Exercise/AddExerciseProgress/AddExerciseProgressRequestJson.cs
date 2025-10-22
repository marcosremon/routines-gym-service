namespace RoutinesGymService.Transversal.JsonInterchange.Exercise.AddExerciseProgress
{
    public class AddExerciseProgressRequestJson
    {
        public List<string> ProgressList { get; set; } = new List<string>();
        public string UserEmail { get; set; } = string.Empty;
        public long RoutineId { get; set; } = -1;
        public long splitDayId { get; set; } = -1;
        public string ExerciseName { get; set; } = string.Empty;
    }
}