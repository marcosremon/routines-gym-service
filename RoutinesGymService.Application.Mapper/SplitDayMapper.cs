using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Application.Mapper
{
    public static class SplitDayMapper
    {
        public static SplitDayDTO SplitDayToDto(SplitDay splitDay)
        {
            return new SplitDayDTO
            {
                DayName = GenericUtils.ChangeStringToEnumOnDayName(splitDay.DayName),
                RoutineId = splitDay.RoutineId,
                DayExercisesDescription = splitDay.DayExercisesDescription,
                Exercises = splitDay.Exercises.Select(e => ExerciseMapper.ExerciseToDto(e)).ToList()
            };
        }
    }
}