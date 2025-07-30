using RoutinesGymService.Domain.Model.Enums;

namespace RoutinesGymService.Application.DataTransferObject.Entity
{
    public class ExerciseProgressDTO
    {
        public long ExerciseId { get; set; }
        public long RoutineId { get; set; }
        public WeekDay DayName { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public float Weight { get; set; }
        public DateTime PerformedAt { get; set; }
    }
}