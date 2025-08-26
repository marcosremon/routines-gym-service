using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.UpdateExercise
{
    public class UpdateExerciseResponse : BaseResponse
    {
        public UserDTO? UserDTO { get; set; } = new UserDTO();
    }
}