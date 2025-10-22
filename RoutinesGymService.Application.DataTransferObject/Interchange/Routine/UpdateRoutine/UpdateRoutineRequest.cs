using RoutinesGymService.Application.DataTransferObject.Entity;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Routine.UpdateRoutine
{
    public class UpdateRoutineRequest
    {
        public long RoutineId { get; set; }
        public string RoutineName { get; set; } = string.Empty;
        public string RoutineDescription { get; set; } = string.Empty;
        public List<SplitDayDTO> SplitDays { get; set; } = new List<SplitDayDTO>();
    }
}