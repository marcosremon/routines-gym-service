using RoutinesGymService.Domain.Model.Enums;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.UpdateExercise
{
    public class UpdateExerciseRequest
    {
        public long UserId { get; set; }
        public long RoutineId { get; set; }
        public WeekDay DayName { get; set; }
        public string? ExerciseName { get; set; }
        public int? Sets { get; set; }
        public int? Reps { get; set; }
        public double? Weight { get; set; }
    }
}