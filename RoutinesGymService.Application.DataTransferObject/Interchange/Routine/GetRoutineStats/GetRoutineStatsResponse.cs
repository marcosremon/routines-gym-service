using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineStats
{
    public class GetRoutineStatsResponse : BaseResponse
    {
        public int routinesCount { get; set; }
        public int exercisesCount { get; set; }
        public int splitsCount { get; set; }
    }
}