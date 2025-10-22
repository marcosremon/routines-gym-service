using RoutinesGymService.Domain.Model.Enums;

namespace RoutinesGymService.Application.DataTransferObject.Entity
{
    public class ExerciseProgressDTO
    {
        public long ExerciseId { get; set; } = -1;
        public long RoutineId { get; set; } = -1;
        public WeekDay DayName { get; set; } = WeekDay.NONE;
        public int Sets { get; set; } = 0;
        public int Reps { get; set; } = 0;
        public float Weight { get; set; } = 0f;
        public DateTime PerformedAt { get; set; }
    }
}