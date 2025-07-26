using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Transversal.JsonInterchange.Routine.GetRoutineStats
{
    public class GetRoutineStatsResponseJson : BaseResponseJson
    {
        public int routinesCount { get; set; }
        public int exercisesCount { get; set; }
        public int splitsCount { get; set; }
    }
}