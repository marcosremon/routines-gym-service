namespace RoutinesGymService.Transversal.JsonInterchange.Exercise.AddExerciseProgress
{
    public class AddExerciseAddExerciseProgressRequestJson
    {
        public List<string> ProgressList { get; set; } = new List<string>();
        public string UserEmail { get; set; } = string.Empty;
        public int RoutineId { get; set; }
        public string DayName { get; set; } = string.Empty;
    }
}