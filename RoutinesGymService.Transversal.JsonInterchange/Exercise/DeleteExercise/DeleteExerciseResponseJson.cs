using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Transversal.JsonInterchange.Exercise.DeleteExercise
{
    public class DeleteExerciseResponseJson : BaseResponseJson
    {
        public UserDTO? UserDTO { get; set; } = new UserDTO();
    }
}