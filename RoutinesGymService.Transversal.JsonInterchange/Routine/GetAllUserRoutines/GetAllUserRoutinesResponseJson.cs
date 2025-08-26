using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Routine.GetAllUserRoutines
{
    public class GetAllUserRoutinesResponseJson : BaseResponseJson
    {
        public List<RoutineDTO> Routines { get; set; } = new List<RoutineDTO>();
    }
}