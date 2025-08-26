using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Exercise.GetExercisesByDayAndRoutineId
{
    public class GetExercisesByDayAndRoutineNameResponseJson : BaseResponseJson
    {
        public List<ExerciseDTO> Exercises { get; set; } = new List<ExerciseDTO>();
        public Dictionary<long, List<string>> PastProgress { get; set; } = new();
    }
}