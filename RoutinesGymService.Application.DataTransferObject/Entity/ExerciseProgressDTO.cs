using RoutinesGymService.Domain.Model.Enums;

namespace RoutinesGymService.Application.DataTransferObject.Entity
{
    public class ExerciseProgressDTO
    {
        public int ExerciseId { get; set; }
        public int RoutineId { get; set; }
        public WeekDay DayName { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public float Weight { get; set; }
        public DateTime PerformedAt { get; set; }
    }
}