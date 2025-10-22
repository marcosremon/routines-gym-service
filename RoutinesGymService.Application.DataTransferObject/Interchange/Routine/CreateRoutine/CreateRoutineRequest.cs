using RoutinesGymService.Application.DataTransferObject.Entity;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Routine.CreateRoutine
{
    public class CreateRoutineRequest
    {
        public string UserEmail { get; set; } = string.Empty;
        public string RoutineName { get; set; } = string.Empty;
        public string RoutineDescription { get; set; } = string.Empty;
        public List<SplitDayDTO> SplitDays { get; set; } = new List<SplitDayDTO>();
    }
}