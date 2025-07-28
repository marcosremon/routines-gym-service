using RoutinesGymService.Application.DataTransferObject.Entity;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Routine.CreateRoutine
{
    public class CreateRoutineRequest
    {
        public string? UserEmail { get; set; }
        public string? RoutineName { get; set; }
        public string? RoutineDescription { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public float Weight { get; set; }
        public List<SplitDayDTO> SplitDays { get; set; } = new List<SplitDayDTO>();
    }
}