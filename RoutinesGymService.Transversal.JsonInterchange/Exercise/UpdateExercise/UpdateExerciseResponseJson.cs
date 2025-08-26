using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Exercise.UpdateExercise
{
    public class UpdateExerciseResponseJson : BaseResponseJson
    {
        public UserDTO? UserDTO { get; set; } = new UserDTO();
    }
}