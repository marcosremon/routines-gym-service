using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineStats
{
    public class GetRoutineStatsResponse : BaseResponse
    {
        public int RoutinesCount { get; set; } = 0;
        public int ExercisesCount { get; set; } = 0;
        public int SplitsCount { get; set; } = 0;
    }
}