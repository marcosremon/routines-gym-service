namespace RoutinesGymService.Application.DataTransferObject.Interchange.Routine.DeleteRoutine
{
    public class DeleteRoutineRequest
    {
        public string? UserEmail { get; set; }
        public long? RoutineId { get; set; }
    }
}