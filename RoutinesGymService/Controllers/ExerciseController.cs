using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExercise;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.AddExerciseProgress;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.DeleteExercise;
using RoutinesGymService.Application.DataTransferObject.Interchange.Exercise.GetExercisesByDayAndRoutineId;
using RoutinesGymService.Transversal.JsonInterchange.Exercise.AddExercise;
using RoutinesGymService.Transversal.JsonInterchange.Exercise.DeleteExercise;
using RoutinesGymService.Transversal.JsonInterchange.Exercise.GetExercisesByDayAndRoutineId;
using RoutinesGymService.Transversal.JsonInterchange.Exercise.UpdateExercise;
using TFC.Application.DTO.Exercise.UpdateExercise;
using TFC.Application.Interface.Application;
using TFC.Transversal.Logs;

namespace RoutinesGymService.Controllers
{
    [ApiController]
    [Route("api/exercise")]
    public class ExerciseController : ControllerBase
    {
        private readonly IExerciseApplication _exerciseApplication;

        public ExerciseController(IExerciseApplication exerciseApplication)
        {
            _exerciseApplication = exerciseApplication;
        }

        [HttpPost("add-exercise-progress")]
        public async Task<ActionResult<AddExerciseAddExerciseProgressResponse>> AddExerciseProgress([FromBody] AddExerciseAddExerciseProgressRequest addExerciseRequest)
        {
            try
            {
                AddExerciseAddExerciseProgressResponse response = await _exerciseApplication.AddExerciseProgress(addExerciseRequest);
                if (response.IsSuccess)
                {
                    Log.Instance.Trace($"Ejercicio añadido correctamente al usuario con id: {response.UserDTO?.UserId}");
                    return Created(string.Empty, response);
                }

                Log.Instance.Trace($"Error al añadir el ejercicio: {response?.Message}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"AddExercise --> Error al añadir el ejercicio: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("update-exercise")]
        public async Task<ActionResult<UpdateExerciseResponse>> UpdateExercise([FromBody] UpdateExerciseRequest updateExerciseRequest)
        {
            try
            {
                UpdateExerciseResponse response = await _exerciseApplication.UpdateExercise(updateExerciseRequest);
                if (response.IsSuccess)
                {
                    Log.Instance.Trace($"Ejercicio actualizado correctamente para el usuario con id: {response.UserDTO?.UserId}");
                    return Ok(response);
                }

                Log.Instance.Trace($"Error al actualizar el ejercicio: {response?.Message}");
                return BadRequest(response?.Message);
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"UpdateExercise --> Error al actualizar el ejercicio: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("delete-exercise")]
        public async Task<ActionResult<DeleteExerciseResponse>> DeleteExercise([FromBody] DeleteExerciseRequest deleteExerciseRequest)
        {
            try
            {
                DeleteExerciseResponse response = await _exerciseApplication.DeleteExercise(deleteExerciseRequest);
                if (response.IsSuccess)
                {
                    Log.Instance.Trace($"Ejercicio eliminado correctamente para el usuario con id: {response.UserDTO?.UserId}");
                    return Ok(response);
                }

                Log.Instance.Trace($"Error al eliminar el ejercicio: {response?.Message}");
                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"DeleteExercise --> Error al eliminar el ejercicio: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("add-exercise")]
        public async Task<ActionResult<AddExerciseResponse>> addExercise([FromBody] AddExerciseRequest addExerciseRequest)
        {
            try
            {
                AddExerciseResponse response = await _exerciseApplication.addExercise(addExerciseRequest);
                if (response.IsSuccess)
                {
                    Log.Instance.Trace($"Ejercicio creado correctamente para el usuario con id: {response.UserDTO?.UserId}");
                    return Ok(response);
                }

                Log.Instance.Trace($"Error al eliminar el ejercicio: {response?.Message}");
                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"DeleteExercise --> Error al eliminar el ejercicio: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("get-exercises-by-day-and-routine-id")]
        public async Task<ActionResult<GetExercisesByDayAndRoutineIdResponse>> GetExercisesByDayAndRoutineId([FromBody] GetExercisesByDayAndRoutineIdRequest getExercisesByDayNameRequest)
        {
            try
            {
                GetExercisesByDayAndRoutineIdResponse response = await _exerciseApplication.GetExercisesByDayAndRoutineId(getExercisesByDayNameRequest);
                if (response.IsSuccess)
                {
                    Log.Instance.Trace($"Ejercicios obtenidos correctamente");
                    return Ok(response);
                }

                Log.Instance.Trace($"Error al obtener los ejercicios: {response?.Message}");
                return BadRequest(response?.Message);
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"GetExercisesByDayName --> Error al obtener los ejercicios: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}