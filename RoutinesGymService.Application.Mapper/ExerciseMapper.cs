using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Transversal.Common.Utils;

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
    
        public static ExerciseProgressDTO ExerciseProgressToDto(ExerciseProgress exerciseProgress)
        {
            return new ExerciseProgressDTO
            {
                ExerciseId = exerciseProgress.ExerciseId,
                RoutineId = exerciseProgress.RoutineId,
                DayName = GenericUtils.ChangeStringToEnumOnDayName(exerciseProgress.DayName),
                Sets = exerciseProgress.Sets,
                Reps = exerciseProgress.Reps,
                Weight = exerciseProgress.Weight,
                PerformedAt = exerciseProgress.PerformedAt
            };
        }
    }
}