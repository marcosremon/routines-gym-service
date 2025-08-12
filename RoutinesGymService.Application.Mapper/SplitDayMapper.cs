using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Domain.Model.Enums;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Application.Mapper
{
    public static class SplitDayMapper
    {
        public static SplitDayDTO SplitDayToDto(SplitDay splitDay)
        {
            return new SplitDayDTO
            {
                DayName = GenericUtils.ChangeIntToEnumOnDayName(splitDay.DayName),
                RoutineId = splitDay.RoutineId,
                DayExercisesDescription = splitDay.DayExercisesDescription,
                Exercises = splitDay.Exercises.Select(e => ExerciseMapper.ExerciseToDto(e)).ToList()
            };
        }

        public static SplitDay SplitDayDtoToEntity(SplitDayDTO splitDayDto)
        {
            return new SplitDay
            {
                DayName = GenericUtils.ChangeEnumToIntOnDayName(splitDayDto.DayName),
                DayNameString = splitDayDto.DayName.ToString().ToUpper(),
                DayExercisesDescription = splitDayDto.DayExercisesDescription,
                Exercises = splitDayDto.Exercises
                    .Select(e => ExerciseMapper.ExerciseDtoToEntity(e))
                    .ToList()
            };
        }
    }
}