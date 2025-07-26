using Microsoft.AspNetCore.Mvc;
using TFC.Application.Interface.Application;

namespace RoutinesGymService.Controllers
{
    [ApiController]
    [Route("routine")]
    public class RoutineController : ControllerBase
    {
        private readonly IRoutineApplication _routineApplication;

        public RoutineController(IRoutineApplication routineApplication)
        {
            _routineApplication = routineApplication;
        }

        #region create-routine
        [HttpPost("create-routine")]
        public async Task<ActionResult<CreateRoutineResponse>> CreateRoutine([FromBody] CreateRoutineRequest createRoutineRequest)
        {
            try
            {
                CreateRoutineResponse response = await _routineApplication.CreateRoutine(createRoutineRequest);
                if (response.IsSuccess)
                {
                    Log.Instance.Trace($"Rutina añadida correctamente al usuario con email: {createRoutineRequest.UserEmail}");
                    return Created(string.Empty, response);
                }

                Log.Instance.Trace($"Error al añadir la rutina: {response?.Message}");
                return BadRequest(response?.Message);
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"CreateRoutine --> Error al añadir la rutina: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region update-routine
        [HttpPost("update-routine")]
        public async Task<ActionResult<UpdateRoutineResponse>> UpdateRoutine([FromBody] UpdateRoutineRequest updateRoutineRequest)
        {
            try
            {
                UpdateRoutineResponse response = await _routineApplication.UpdateUser(updateRoutineRequest);
                if (response.IsSuccess)
                {
                    Log.Instance.Trace($"Rutina con nombre: {updateRoutineRequest.RoutineName} actualizada correctamente");
                    return Ok(response);
                }

                Log.Instance.Trace($"Error al actualizar la rutina: {response?.Message}");
                return BadRequest(response?.Message);
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"UpdateRoutine --> Error al actualizar la rutina: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region delete-routine
        [HttpPost("delete-routine")]
        public async Task<ActionResult<DeleteRoutineResponse>> DeleteRoutine([FromBody] DeleteRoutineRequest deleteRoutineRequest)
        {
            try
            {
                DeleteRoutineResponse response = await _routineApplication.DeleteRoutine(deleteRoutineRequest);
                if (response.IsSuccess)
                {
                    Log.Instance.Trace($"Rutina con id: {deleteRoutineRequest.RoutineId} eliminada correctamente");
                    return NoContent();
                }

                Log.Instance.Trace($"Error al eliminar la rutina: {response?.Message}");
                return BadRequest(response?.Message);
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"DeleteRoutine --> Error al eliminar la rutina: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region get-all-user-routines
        [HttpPost("get-all-user-routines")]
        public async Task<ActionResult<GetAllUserRoutinesResponse>> GetAllUserRoutines([FromBody] GetAllUserRoutinesRequest getAllUserRoutinesRequest)
        {
            try
            {
                GetAllUserRoutinesResponse response = await _routineApplication.GetAllUserRoutines(getAllUserRoutinesRequest);
                if (response.IsSuccess)
                {
                    Log.Instance.Trace($"Rutinas del usuario obtenidas correctamente");
                    return Ok(response);
                }

                Log.Instance.Trace($"Error al obtener las rutinas: {response?.Message}");
                return BadRequest(response?.Message);
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"GetAllUserRoutines --> Error al obtener las rutinas: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region get-routine-stats
        [HttpPost("get-routine-stats")]
        public async Task<ActionResult<GetRoutineStatsResponse>> GetRoutineStats([FromBody] GetRoutineStatsRequest getRoutineStatsRequest)
        {
            try
            {
                GetRoutineStatsResponse response = await _routineApplication.GetRoutineStats(getRoutineStatsRequest);
                if (response.IsSuccess)
                {
                    Log.Instance.Trace($"Estadisticas de las rutinas obtenidas correctamente");
                    return Ok(response);
                }

                Log.Instance.Trace($"Error al obtener las estadisticas de las rutinas: {response?.Message}");
                // Devuelve el objeto de respuesta aunque sea error
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"GetRoutineStats --> Error al obtener las estadisticas de las rutinas: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region get-routine-by-id
        [HttpPost("get-routine-by-id")]
        public async Task<ActionResult<GetRoutineByIdResponse>> GetRoutineById([FromBody] GetRoutineByIdRequest getRoutineByIdRequest)
        {
            try
            {
                GetRoutineByIdResponse response = await _routineApplication.GetRoutineById(getRoutineByIdRequest);
                if (response.IsSuccess)
                {
                    Log.Instance.Trace($"Rutina con id: {getRoutineByIdRequest.RoutineId} obtenida correctamente");
                    return Ok(response);
                }

                Log.Instance.Trace($"Error al obtener la rutina: {response?.Message}");
                return BadRequest(response?.Message);
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"GetRoutineById --> Error al obtener la rutina: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}