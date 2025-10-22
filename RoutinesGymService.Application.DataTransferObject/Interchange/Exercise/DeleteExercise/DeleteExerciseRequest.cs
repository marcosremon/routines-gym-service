namespace RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.DeleteExercise
{
    public class DeleteExerciseRequest
    {
        public string UserEmail { get; set; } = string.Empty;
        public long RoutineId { get; set; } = -1;
        public string DayName { get; set; } = string.Empty; 
        public string ExerciseName { get; set; } = string.Empty;
    }
}