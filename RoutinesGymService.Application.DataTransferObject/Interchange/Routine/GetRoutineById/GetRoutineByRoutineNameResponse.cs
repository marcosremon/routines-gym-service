using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineById
{
    public class GetRoutineByRoutineNameResponse : BaseResponse
    {
        public RoutineDTO? RoutineDTO { get; set; }
    }
}