using TFC.Application.DTO.Exercise.UpdateExercise;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExercise;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExerciseProgress;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.DeleteExercise;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.GetExercisesByDayAndRoutineId;

namespace TFC.Application.Interface.Application
{
    public interface IExerciseApplication
    {
        Task<AddExerciseResponse> AddExercise(AddExerciseRequest addExerciseRequest);
        Task<AddExerciseAddExerciseProgressResponse> AddExerciseProgress(AddExerciseAddExerciseProgressRequest addExerciseRequest);
        Task<DeleteExerciseResponse> DeleteExercise(DeleteExerciseRequest deleteExerciseRequest);
        Task<GetExercisesByDayAndRoutineIdResponse> GetExercisesByDayAndRoutineId(GetExercisesByDayAndRoutineIdRequest getExercisesByDayNameRequest);
        Task<UpdateExerciseResponse> UpdateExercise(UpdateExerciseRequest updateExerciseRequest);
    }
}