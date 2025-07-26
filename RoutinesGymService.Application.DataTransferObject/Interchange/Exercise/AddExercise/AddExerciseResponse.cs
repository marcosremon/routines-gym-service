using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExercise
{
    public class AddExerciseResponse : BaseResponse
    {
        public UserDTO? UserDTO { get; set; }
    }
}