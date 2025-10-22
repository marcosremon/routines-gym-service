using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Routine.UpdateRoutine
{
    public class UpdateRoutineResponse : BaseResponse
    {
        public RoutineDTO RoutineDto { get; set; } = new RoutineDTO();
    }
}