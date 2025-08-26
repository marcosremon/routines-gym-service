using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Routine.GetRoutineStats
{
    public class GetRoutineStatsResponseJson : BaseResponseJson
    {
        public int RoutinesCount { get; set; }
        public int ExercisesCount { get; set; }
        public int SplitsCount { get; set; }
    }
}