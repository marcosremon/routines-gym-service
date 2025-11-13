using RoutinesGymService.Domain.Model.Enums;

namespace RoutinesGymService.Application.DataTransferObject.SplitDay.DeleteSplitDay
{
    public class DeleteSplitDayRequest
    {
        public WeekDay DayName { get; set; } = WeekDay.NONE;
        public long RoutineId { get; set; }
        public long UserId { get; set; }
    }
}