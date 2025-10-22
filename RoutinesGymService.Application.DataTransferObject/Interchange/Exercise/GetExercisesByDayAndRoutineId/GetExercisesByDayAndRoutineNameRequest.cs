namespace RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.GetExercisesByDayAndRoutineId
{
    public class GetExercisesByDayAndRoutineNameRequest
    {
        public string RoutineName { get; set; } = string.Empty;
        public string DayName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }
}