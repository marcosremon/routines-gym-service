using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.GetExercisesByDayAndRoutineId
{
    public class GetExercisesByDayAndRoutineNameResponse : BaseResponse
    {
        public List<ExerciseDTO> Exercises { get; set; } = new List<ExerciseDTO>();
        public Dictionary<long, List<string>> PastProgress { get; set; } = new();
    }
}