using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExerciseProgress
{
    public class AddExerciseAddExerciseProgressResponse : BaseResponse
    {
        public UserDTO? UserDTO { get; set; } = new UserDTO();
    }
}