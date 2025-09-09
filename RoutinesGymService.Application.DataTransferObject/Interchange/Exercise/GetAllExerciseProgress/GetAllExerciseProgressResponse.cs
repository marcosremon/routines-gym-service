using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.GetAllExerciseProgress
{
    public class GetAllExerciseProgressResponse : BaseResponse
    {
        public List<ExerciseProgressDTO> ExerciseProgressList { get; set; } = new List<ExerciseProgressDTO>();
    }
}