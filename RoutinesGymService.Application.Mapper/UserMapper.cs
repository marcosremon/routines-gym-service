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
                Role = user.Role == 0 ? Role.USER : Role.ADMIN,
                InscriptionDate = user.InscriptionDate.ToString("yyyy-MM-dd")
            };
        }
    }
}