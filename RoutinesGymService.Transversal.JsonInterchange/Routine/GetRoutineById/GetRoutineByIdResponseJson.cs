using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Transversal.JsonInterchange.Routine.GetRoutineById
{
    public class GetRoutineByIdResponseJson : BaseResponseJson
    {
        public RoutineDTO? RoutineDTO { get; set; }
    }
}