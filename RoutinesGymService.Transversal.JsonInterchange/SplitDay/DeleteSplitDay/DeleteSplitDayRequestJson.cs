using RoutinesGymService.Domain.Model.Enums;

namespace RoutinesGymService.Transversal.JsonInterchange.SplitDay.DeleteSplitDay
{
    public class DeleteSplitDayRequestJson
    {
        public WeekDay DayName { get; set; } = WeekDay.NONE;
        public long RoutineId { get; set; }
        public long UserId { get; set; }
    }
}