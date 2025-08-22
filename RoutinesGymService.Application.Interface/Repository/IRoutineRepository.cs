using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.CreateRoutine;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.DeleteRoutine;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetAllUserRoutines;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineById;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineStats;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.UpdateRoutine;

namespace RoutinesGymService.Application.Interface.Repository
{
    public interface IRoutineRepository
    {
        Task<CreateRoutineResponse> CreateRoutine(CreateRoutineRequest createRoutineRequest);
        Task<DeleteRoutineResponse> DeleteRoutine(DeleteRoutineRequest deleteRoutineRequest);
        Task<GetAllUserRoutinesResponse> GetAllUserRoutines(GetAllUserRoutinesRequest getAllUserRoutinesRequest);
        Task<GetRoutineByRoutineNameResponse> GetRoutineByRoutineName(GetRoutineByRoutineNameRequest getRoutineByRoutineNameRequest);
        Task<GetRoutineStatsResponse> GetRoutineStats(GetRoutineStatsRequest getRoutineStatsRequest);
        Task<UpdateRoutineResponse> UpdateRoutine(UpdateRoutineRequest updateRoutineRequest);
    }
}