namespace RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.ExerciseExistInThisWeekday
{
    public class ExerciseExistInThisWeekdayRequest
    {
        public string RoutineName { get; set; } = string.Empty;
        public string ExerciseName { get; set; } = string.Empty;
        public string DayName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }
}