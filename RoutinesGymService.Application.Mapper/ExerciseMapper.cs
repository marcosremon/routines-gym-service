using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Domain.Model.Entities;

namespace RoutinesGymService.Application.Mapper
{
    public static class ExerciseMapper
    {
        public static ExerciseDTO ExerciseToDto(Exercise exercise)
        {
            return new ExerciseDTO
            {
                ExerciseName = exercise.ExerciseName,
                RoutineId = exercise.RoutineId,
                SplitDayId = exercise.SplitDayId,
            };
        }

        public static Exercise ExerciseDtoToEntity(ExerciseDTO exerciseDto)
        {
            return new Exercise
            {
                ExerciseName = exerciseDto.ExerciseName,
            };
        }
    }
}