using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Transversal.Common.Utils;

namespace RoutinesGymService.Application.Mapper
{
    public static class RoutineMapper
    {
        public static RoutineDTO RoutineToDto(Routine routine)
        {
            return new RoutineDTO
            {
                RoutineName = routine.RoutineName ?? string.Empty,
                RoutineDescription = routine.RoutineDescription ?? string.Empty,
                SplitDays = routine.SplitDays.Select(sd => new SplitDayDTO
                {
                    DayName = GenericUtils.ChangeIntToEnumOnDayName(sd.DayName),
                    RoutineId = routine.RoutineId,
                    DayExercisesDescription = sd.DayExercisesDescription ?? string.Empty,
                    Exercises = sd.Exercises.Select(exercise => new ExerciseDTO
                    {
                        ExerciseName = exercise.ExerciseName ?? string.Empty,
                        RoutineId = routine.RoutineId,
                        SplitDayId = sd.SplitDayId,
                    }).ToList()
                }).ToList()
            };
        }
    }
}