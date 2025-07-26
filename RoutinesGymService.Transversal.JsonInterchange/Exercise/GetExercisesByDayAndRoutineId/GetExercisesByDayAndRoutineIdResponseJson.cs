using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Transversal.JsonInterchange.Exercise.GetExercisesByDayAndRoutineId
{
    public class GetExercisesByDayAndRoutineIdResponseJson : BaseResponseJson
    {
        public List<ExerciseDTO> Exercises { get; set; } = new List<ExerciseDTO>();
        public Dictionary<long, List<string>> PastProgress { get; set; } = new();
    }
}