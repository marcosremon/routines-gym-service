namespace RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.GetExercisesByDayAndRoutineId
{
    public class GetExercisesByDayAndRoutineNameRequest
    {
        public string? RoutineName { get; set; }
        public string? DayName { get; set; }
        public string? UserEmail { get; set; }
    }
}