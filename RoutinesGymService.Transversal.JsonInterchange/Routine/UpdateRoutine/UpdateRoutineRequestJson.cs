using RoutinesGymService.Application.DataTransferObject.Entity;

namespace RoutinesGymService.Transversal.JsonInterchange.Routine.UpdateRoutine
{
    public class UpdateRoutineRequestJson
    {
        public long RoutineId { get; set; }
        public string? RoutineName { get; set; }
        public string? RoutineDescription { get; set; }
        public List<SplitDayDTO> SplitDays { get; set; } = new List<SplitDayDTO>();
    }
}