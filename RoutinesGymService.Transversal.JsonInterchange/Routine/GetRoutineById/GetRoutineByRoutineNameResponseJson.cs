using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Routine.GetRoutineById
{
    public class GetRoutineByRoutineNameResponseJson : BaseResponseJson
    {
        public RoutineDTO? RoutineDTO { get; set; }
    }
}