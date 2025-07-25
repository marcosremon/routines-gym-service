namespace RoutinesGymService.Application.DataTransferObject.Entity
{
    public class RoutineDTO
    {
        public string RoutineName { get; set; } = string.Empty;
        public string RoutineDescription { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}