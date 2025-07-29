using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineStats
{
    public class GetRoutineStatsResponse : BaseResponse
    {
        public int RoutinesCount { get; set; }
        public int ExercisesCount { get; set; }
        public int SplitsCount { get; set; }
    }
}