namespace RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineById
{
    public class GetRoutineByRoutineNameRequest
    {
        public string RoutineName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }
}