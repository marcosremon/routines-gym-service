using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Domain.Model.Enums;

namespace RoutinesGymService.Application.Mapper
{
    public static class UserMapper
    {
        public static UserDTO UserToDto(User user)
        {
            return new UserDTO
            {
                Dni = user.Dni,
                Username = user.Username,
                Surname = user.Surname,
                Email = user.Email,
                FriendCode = user.FriendCode,
                Password = "***************",
                Role = user.Role.ToLower() == "user" ? Role.USER : Role.ADMIN,
                InscriptionDate = user.InscriptionDate.ToString("yyyy-MM-dd")
            };
        }

        //public static UserDTO UserToDtoCompleat(User user, RoutineDTO routineDto)
        //{
        //    return new UserDTO
        //    {
        //        UserId = user.UserId,
        //        Username = user.Username,
        //        FriendCode = user.FriendCode,
        //        Email = user.Email,
        //        Routines = user.Routines.Select(r => new RoutineDTO
        //        {
        //            RoutineId = r.RoutineId,
        //            RoutineName = r.RoutineName,
        //            SplitDays = r.SplitDays.Select(sd => new SplitDayDTO
        //            {
        //                DayName = sd.DayName,
        //                Exercises = sd.Exercises.Select(e => new ExerciseDTO
        //                {
        //                    ExerciseId = e.ExerciseId,
        //                    ExerciseName = e.ExerciseName,
        //                    Reps = e.Reps,
        //                    Sets = e.Sets
        //                }).ToList()
        //            }).ToList()
        //        }).ToList()
        //    };
        //}
    }
}