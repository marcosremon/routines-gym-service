using RoutinesGymService.Application.DataTransferObject.Entity;

namespace RoutinesGymService.Transversal.JsonInterchange.Routine.CreateRoutine
{
    public class CreateRoutineRequestJson
    {
        public string UserEmail { get; set; } = string.Empty;
        public string RoutineName { get; set; } = string.Empty;
        public string RoutineDescription { get; set; } = string.Empty;
        public List<SplitDayDTO> SplitDays { get; set; } = new List<SplitDayDTO>();
    }
}