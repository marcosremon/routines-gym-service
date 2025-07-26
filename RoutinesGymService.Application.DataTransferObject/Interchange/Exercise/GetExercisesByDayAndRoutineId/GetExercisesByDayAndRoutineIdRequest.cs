namespace RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.GetExercisesByDayAndRoutineId
{
    public class GetExercisesByDayAndRoutineIdRequest
    {
        public int? RoutineId { get; set; }
        public string? DayName { get; set; }
    }
}