using RoutinesGymService.Domain.Model.Enums;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.SplitDay.GetRoutineSplits
{
    public class DayInfoDTO
    {
        public WeekDay WeekDay { get; set; } = WeekDay.NONE;
        public string DayExercisesDescription { get; set; } = string.Empty; // Pecho, Espalda, Torso...
    }
}