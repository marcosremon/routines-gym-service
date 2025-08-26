using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetAllUserRoutines
{
    public class GetAllUserRoutinesResponse : BaseResponse
    {
        public List<RoutineDTO> Routines { get; set; } = new List<RoutineDTO>();
    }
}