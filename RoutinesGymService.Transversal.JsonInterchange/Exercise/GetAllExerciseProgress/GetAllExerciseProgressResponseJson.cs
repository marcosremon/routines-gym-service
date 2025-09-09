using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Exercise.GetAllExerciseProgress
{
    public class GetAllExerciseProgressResponseJson : BaseResponseJson
    {
        public List<ExerciseProgressDTO> ExerciseProgressList { get; set; } = new List<ExerciseProgressDTO>();
    }
}