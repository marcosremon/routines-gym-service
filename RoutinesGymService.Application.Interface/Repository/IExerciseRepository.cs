using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExercise;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExerciseProgress;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.DeleteExercise;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.GetExercisesByDayAndRoutineId;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.UpdateExercise;

namespace RoutinesGymService.Application.Interface.Repository
{
    public interface IExerciseRepository
    {
        Task<AddExerciseResponse> addExercise(AddExerciseRequest addExerciseRequest);
        Task<AddExerciseAddExerciseProgressResponse> AddExerciseProgress(AddExerciseAddExerciseProgressRequest addExerciseRequest);
        Task<DeleteExerciseResponse> DeleteExercise(DeleteExerciseRequest deleteExerciseRequest);
        Task<GetExercisesByDayAndRoutineIdResponse> GetExercisesByDayAndRoutineId(GetExercisesByDayAndRoutineIdRequest getExercisesByDayNameRequest);
        Task<UpdateExerciseResponse> UpdateExercise(UpdateExerciseRequest updateExerciseRequest);
    }
}