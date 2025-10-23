using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.ExerciseExistInThisWeekday
{
    public class ExerciseExistInThisWeekdayResponse : BaseResponse
    {
        public long RoutineId { get; set; } = -1;
        public long SplitDayId { get; set; } = -1;
    }
}