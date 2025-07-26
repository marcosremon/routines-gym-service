using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.CreateRoutine;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.DeleteRoutine;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetAllUserRoutines;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineById;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineStats;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.UpdateRoutine;

namespace RoutinesGymService.Application.Interface.Application
{
    public interface IRoutineApplication
    {
        Task<CreateRoutineResponse> CreateRoutine(CreateRoutineRequest createRoutineRequest);
        Task<DeleteRoutineResponse> DeleteRoutine(DeleteRoutineRequest deleteRoutineRequest);
        Task<GetAllUserRoutinesResponse> GetAllUserRoutines(GetAllUserRoutinesRequest getAllUserRoutinesRequest);
        Task<GetRoutineByIdResponse> GetRoutineById(GetRoutineByIdRequest getRoutineByIdRequest);
        Task<GetRoutineStatsResponse> GetRoutineStats(GetRoutineStatsRequest getRoutineStatsRequest);
        Task<UpdateRoutineResponse> UpdateUser(UpdateRoutineRequest updateRoutineRequest);
    }
}