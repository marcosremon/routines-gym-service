using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.CreateRoutine;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.DeleteRoutine;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetAllUserRoutines;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineById;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineStats;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.UpdateRoutine;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Application.Interface.Repository;

namespace RoutinesGymService.Application.UseCase
{
    public class RoutineApplication : IRoutineApplication
    {
        private readonly IRoutineRepository _routineRepository;

        public RoutineApplication(IRoutineRepository routineRepository)
        {
            _routineRepository = routineRepository;
        }

        public async Task<CreateRoutineResponse> CreateRoutine(CreateRoutineRequest createRoutineRequest)
        {
            return await _routineRepository.CreateRoutine(createRoutineRequest);
        }

        public async Task<DeleteRoutineResponse> DeleteRoutine(DeleteRoutineRequest deleteRoutineRequest)
        {
            return await _routineRepository.DeleteRoutine(deleteRoutineRequest);
        }

        public async Task<GetAllUserRoutinesResponse> GetAllUserRoutines(GetAllUserRoutinesRequest getAllUserRoutinesRequest)
        {
            return await _routineRepository.GetAllUserRoutines(getAllUserRoutinesRequest);
        }

        public async Task<GetRoutineByRoutineNameResponse> GetRoutineByRoutineName(GetRoutineByRoutineNameRequest getRoutineByRoutineNameRequest)
        {
            return await _routineRepository.GetRoutineByRoutineName(getRoutineByRoutineNameRequest);
        }

        public async Task<GetRoutineStatsResponse> GetRoutineStats(GetRoutineStatsRequest getRoutineStatsRequest)
        {
            return await _routineRepository.GetRoutineStats(getRoutineStatsRequest);
        }

        public async Task<UpdateRoutineResponse> UpdateUser(UpdateRoutineRequest updateRoutineRequest)
        {
            return await _routineRepository.UpdateRoutine(updateRoutineRequest);
        }
    }
}