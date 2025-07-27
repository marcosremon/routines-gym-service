using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExercise;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExerciseProgress;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.DeleteExercise;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.GetExercisesByDayAndRoutineId;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.UpdateExercise;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Infraestructure.Persistence.Context;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly ApplicationDbContext _context;

        public ExerciseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AddExerciseResponse> addExercise(AddExerciseRequest addExerciseRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<AddExerciseAddExerciseProgressResponse> AddExerciseProgress(AddExerciseAddExerciseProgressRequest addExerciseRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<DeleteExerciseResponse> DeleteExercise(DeleteExerciseRequest deleteExerciseRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<GetExercisesByDayAndRoutineIdResponse> GetExercisesByDayAndRoutineId(GetExercisesByDayAndRoutineIdRequest getExercisesByDayNameRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<UpdateExerciseResponse> UpdateExercise(UpdateExerciseRequest updateExerciseRequest)
        {
            throw new NotImplementedException();
        }
    }
}