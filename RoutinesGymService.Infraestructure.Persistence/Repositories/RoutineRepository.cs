using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.CreateRoutine;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.DeleteRoutine;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetAllUserRoutines;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineById;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineStats;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.UpdateRoutine;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Infraestructure.Persistence.Context;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class RoutineRepository : IRoutineRepository
    {
        private readonly ApplicationDbContext _context;

        public RoutineRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CreateRoutineResponse> CreateRoutine(CreateRoutineRequest createRoutineRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<DeleteRoutineResponse> DeleteRoutine(DeleteRoutineRequest deleteRoutineRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<GetAllUserRoutinesResponse> GetAllUserRoutines(GetAllUserRoutinesRequest getAllUserRoutinesRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<GetRoutineByIdResponse> GetRoutineById(GetRoutineByIdRequest getRoutineByIdRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<GetRoutineStatsResponse> GetRoutineStats(GetRoutineStatsRequest getRoutineStatsRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<UpdateRoutineResponse> UpdateRoutine(UpdateRoutineRequest updateRoutineRequest)
        {
            throw new NotImplementedException();
        }
    }
}