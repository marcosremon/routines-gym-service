using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Transversal.JsonInterchange.Exercise.AddExerciseProgress
{
    public class AddExerciseAddExerciseProgressResponseJson : BaseResponseJson
    {
        public UserDTO? UserDTO { get; set; } = new UserDTO();
    }
}