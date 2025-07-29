using RoutinesGymService.Domain.Model.Enums;

namespace RoutinesGymService.Application.DataTransferObject.Entity
{
    public class SplitDayDTO
    {
        public WeekDay DayName { get; set; } 
        public int RoutineId { get; set; }
        public string DayExercisesDescription { get; set; } = string.Empty;
        public List<ExerciseDTO> Exercises { get; set; } = new List<ExerciseDTO>();
    }
}