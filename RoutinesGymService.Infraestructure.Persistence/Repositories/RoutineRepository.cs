using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.CreateRoutine;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.DeleteRoutine;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetAllUserRoutines;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineById;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.GetRoutineStats;
using RoutinesGymService.Application.DataTransferObject.Interchange.Routine.UpdateRoutine;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Application.Mapper;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Infraestructure.Persistence.Context;
using RoutinesGymService.Transversal.Common;

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
            CreateRoutineResponse createRoutineResponse = new CreateRoutineResponse();
            using (ApplicationDbContext context = _context)
            {
                IDbContextTransaction dbContextTransaction = context.Database.BeginTransaction();
                try
                {
                    User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == createRoutineRequest.UserEmail);
                    if (user == null)
                    {
                        createRoutineResponse.Message = "User not found";
                        createRoutineResponse.IsSuccess = false;
                    }
                    else
                    {
                        if (user.Routines.Any(r => r.RoutineName == createRoutineRequest.RoutineName))
                        {
                            createRoutineResponse.Message = "Routine with this name already exists for the user";
                            createRoutineResponse.IsSuccess = false;
                        }
                        else
                        { 
                            Routine routine = new Routine
                            {
                                RoutineName = createRoutineRequest.RoutineName!,
                                RoutineDescription = createRoutineRequest.RoutineDescription ?? "sin descripcion",
                                SplitDays = new List<SplitDay>(),
                            };

                            _context.Routines.Add(routine);
                            await _context.SaveChangesAsync();

                            routine.SplitDays = createRoutineRequest.SplitDays.Select(sd => new SplitDay
                            {
                                DayName = GenericUtils.ChangeStringToEnumOnDayName(sd.DayName),
                                DayExercisesDescription = "Default description",
                                RoutineId = routine.RoutineId,
                                Exercises = sd.Exercises.Select(ex => new Exercise
                                {
                                    ExerciseName = ex.ExerciseName,
                                    RoutineId = routine.RoutineId,
                                    SplitDayId = _context.SplitDays.Last().SplitDayId + 1,
                                }).ToList() ?? new List<Exercise>(),
                            }).ToList() ?? new List<SplitDay>();

                            await _context.SaveChangesAsync();

                            foreach (SplitDay splitDay in routine.SplitDays)
                            {
                                foreach (Exercise exercise in splitDay.Exercises)
                                {
                                    if (string.IsNullOrEmpty(exercise.ExerciseName))
                                    {
                                        createRoutineResponse.Message = "Exercise name cannot be empty";
                                        createRoutineResponse.IsSuccess = false;
                                    }
                                    else
                                    {
                                        ExerciseProgress progress = new ExerciseProgress
                                        {
                                            ExerciseId = exercise.ExerciseId,
                                            RoutineId = routine.RoutineId,
                                            DayName = splitDay.DayName.ToString(),
                                            Sets = createRoutineRequest.Sets,
                                            Reps = createRoutineRequest.Reps,
                                            Weight = createRoutineRequest.Weight,
                                            PerformedAt = DateTime.UtcNow
                                        };

                                        _context.ExerciseProgress.Add(progress);
                                    }
                                }
                            }

                            await _context.SaveChangesAsync();
                            await dbContextTransaction.CommitAsync();

                            createRoutineResponse.IsSuccess = true;
                            createRoutineResponse.RoutineDTO = Application.Mapper.RoutineMapper.RoutineToDto(routine);
                            createRoutineResponse.Message = "Routine created successfully";
                        }
                    }
                }
                catch (Exception ex)
                {
                    await dbContextTransaction.RollbackAsync();
                    createRoutineResponse.Message = $"unexpected error on RoutineRepository -> CreateRoutine {ex.Message}";
                    createRoutineResponse.IsSuccess = false;
                }
            }
        
            return createRoutineResponse;
        }

        public async Task<DeleteRoutineResponse> DeleteRoutine(DeleteRoutineRequest deleteRoutineRequest)
        {
            DeleteRoutineResponse deleteRoutineResponse = new DeleteRoutineResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == deleteRoutineRequest.UserEmail);
                if (user == null)
                {
                    deleteRoutineResponse.IsSuccess = false;
                    deleteRoutineResponse.Message = "User not found";
                }
                else
                {
                    Routine? routine = await _context.Routines.FirstOrDefaultAsync(r => r.RoutineId == deleteRoutineRequest.RoutineId && r.UserId == user.UserId);
                    if (routine == null)
                    {
                        deleteRoutineResponse.IsSuccess = false;
                        deleteRoutineResponse.Message = "Routine not found for the user";
                    }
                    else
                    {
                        if (!user.Routines.Contains(routine))
                        {
                            deleteRoutineResponse.IsSuccess = false;
                            deleteRoutineResponse.Message = "This routine does not belong to the user";
                        }
                        else
                        {
                            _context.Routines.Remove(routine);
                            await _context.SaveChangesAsync();

                            deleteRoutineResponse.IsSuccess = true;
                            deleteRoutineResponse.UserId = user.UserId;
                            deleteRoutineResponse.Message = "Routine deleted successfully";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                deleteRoutineResponse.Message = $"unexpected error on RoutineRepository -> DeleteRoutine {ex.Message}";
                deleteRoutineResponse.IsSuccess = false;
            }

            return deleteRoutineResponse;
        }

        public async Task<GetAllUserRoutinesResponse> GetAllUserRoutines(GetAllUserRoutinesRequest getAllUserRoutinesRequest)
        {
            GetAllUserRoutinesResponse getAllUserRoutinesResponse = new GetAllUserRoutinesResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == getAllUserRoutinesRequest.UserEmail);
                if (user == null)
                {
                    getAllUserRoutinesResponse.IsSuccess = false;
                    getAllUserRoutinesResponse.Message = "User not found";
                }
                else
                {
                    List<Routine> routines = await _context.Routines.Where(r => r.UserId == user.UserId).ToListAsync();
                    if (routines.Count == 0)
                    {
                        getAllUserRoutinesResponse.IsSuccess = false;
                        getAllUserRoutinesResponse.Message = "No routines found for the user";
                    }
                    else
                    {
                        getAllUserRoutinesResponse.Routines = routines.Select(r => RoutineMapper.RoutineToDto(r)).ToList();
                        getAllUserRoutinesResponse.IsSuccess = true;
                        getAllUserRoutinesResponse.Message = "Routines retrieved successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                getAllUserRoutinesResponse.Message = $"unexpected error on RoutineRepository -> GetAllUserRoutines {ex.Message}";
                getAllUserRoutinesResponse.IsSuccess = false;
            }

            return getAllUserRoutinesResponse;
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