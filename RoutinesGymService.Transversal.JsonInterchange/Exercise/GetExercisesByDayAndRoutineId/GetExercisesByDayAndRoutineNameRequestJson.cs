namespace RoutinesGymService.Transversal.JsonInterchange.Exercise.GetExercisesByDayAndRoutineId
{
    public class GetExercisesByDayAndRoutineNameRequestJson
    {
        public string RoutineName { get; set; } = string.Empty;
        public string DayName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }
}