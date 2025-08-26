using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Routine.UpdateRoutine
{
    public class UpdateRoutineResponseJson : BaseResponseJson
    {
        public RoutineDTO? RoutineDTO { get; set; }
    }
}