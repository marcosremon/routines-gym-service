using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Routine.GetRoutineStats
{
    public class GetRoutineStatsResponseJson : BaseResponseJson
    {
        public int RoutinesCount { get; set; } = 0;
        public int ExercisesCount { get; set; } = 0;
        public int SplitsCount { get; set; } = 0;
    }
}