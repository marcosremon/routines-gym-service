using RoutinesGymService.Application.DataTransferObject.Entity;

namespace RoutinesGymService.Transversal.JsonInterchange.Routine.CreateRoutine
{
    public class CreateRoutineRequestJson
    {
        public string? UserEmail { get; set; }
        public string? RoutineName { get; set; }
        public string? RoutineDescription { get; set; }
        public List<SplitDayDTO> SplitDays { get; set; } = new List<SplitDayDTO>();
    }
}