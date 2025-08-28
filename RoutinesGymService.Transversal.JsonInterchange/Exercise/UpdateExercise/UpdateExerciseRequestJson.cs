using RoutinesGymService.Domain.Model.Enums;

namespace RoutinesGymService.Transversal.JsonInterchange.Exercise.UpdateExercise
{
    public class UpdateExerciseRequestJson
    {
        public long UserId { get; set; }
        public long RoutineId { get; set; }
        public WeekDay DayName { get; set; } = 0;
        public string? ExerciseName { get; set; }
        public int? Sets { get; set; }
        public int? Reps { get; set; }
        public double? Weight { get; set; }
    }
}