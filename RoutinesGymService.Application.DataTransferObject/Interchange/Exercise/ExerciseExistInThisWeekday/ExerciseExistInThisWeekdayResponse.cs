using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.ExerciseExistInThisWeekday
{
    public class ExerciseExistInThisWeekdayResponse : BaseResponse
    {
        public long RoutineId { get; set; }
        public long SplitDayId { get; set; }
    }
}