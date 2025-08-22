namespace RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineById
{
    public class GetRoutineByRoutineNameRequest
    {
        public string? RoutineName { get; set; }
        public string? UserEmail { get; set; }
    }
}