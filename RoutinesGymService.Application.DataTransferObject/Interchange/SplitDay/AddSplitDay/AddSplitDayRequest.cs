using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Domain.Model.Enums;

namespace RoutinesGymService.Application.DataTransferObject.SplitDay.AddSplitDay
{
    public class AddSplitDayRequest
    {
        public WeekDay DayName { get; set; } = WeekDay.NONE;
        public long RoutineId { get; set; } = -1;
        public long UserId { get; set; } = -1;
        public ICollection<ExerciseDTO> Exercises { get; set; } = new List<ExerciseDTO>();
    }
}