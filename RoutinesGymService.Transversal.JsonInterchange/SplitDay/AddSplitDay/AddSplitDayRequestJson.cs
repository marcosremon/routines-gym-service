using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Domain.Model.Enums;

namespace RoutinesGymService.Transversal.JsonInterchange.SplitDay.AddSplitDay
{
    public class AddSplitDayRequestJson
    {
        public WeekDay? DayName { get; set; }
        public long RoutineId { get; set; }
        public long UserId { get; set; }
        public ICollection<ExerciseDTO> Exercises { get; set; } = new List<ExerciseDTO>();
    }
}