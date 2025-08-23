namespace RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExerciseProgress
{
    public class AddExerciseAddExerciseProgressRequest
    {
        public List<string> ProgressList { get; set; } = new List<string>();
        public string UserEmail { get; set; } = string.Empty;
        public long? RoutineId { get; set; }
        public long? splitDayId { get; set; }
        public string? ExerciseName { get; set; }
    }
}