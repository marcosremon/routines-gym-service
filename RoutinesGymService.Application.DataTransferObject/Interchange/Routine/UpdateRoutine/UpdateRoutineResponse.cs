using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Routine.UpdateRoutine
{
    public class UpdateRoutineResponse : BaseResponse
    {
        public RoutineDTO? RoutineDTO { get; set; }
    }
}