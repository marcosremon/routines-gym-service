namespace RoutinesGymService.Application.DataTransferObject.Interchange.Routine.DeleteRoutine
{
    public class DeleteRoutineRequest
    {
        public string UserEmail { get; set; } = string.Empty;
        public string RoutineName { get; set; } = string.Empty;
    }
}