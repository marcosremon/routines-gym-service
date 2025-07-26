using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Transversal.JsonInterchange.Routine.CreateRoutine
{
    public class CreateRoutineResponseJson : BaseResponseJson
    {
        public RoutineDTO? RoutineDTO { get; set; }
    }
}