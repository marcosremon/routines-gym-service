using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.ChangePasswordWithPasswordAndEmail;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateGenericUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateGoogleUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateNewPassword;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.DeleteUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUserByEmail;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUsers;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.UpdateUser;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Infraestructure.Persistence.Context;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ChangePasswordWithPasswordAndEmailResponse> ChangePasswordWithPasswordAndEmail(ChangePasswordWithPasswordAndEmailRequest changePasswordWithPasswordAndEmailRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<CreateGoogleUserResponse> CreateGoogleUser(CreateGenericUserRequest createGenericUserRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<CreateNewPasswordResponse> CreateNewPassword(CreateNewPasswordRequest createNewPasswordRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<CreateUserResponse> CreateUser(CreateGenericUserRequest createGenericUserRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest deleteUserRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<GetUserByEmailResponse> GetUserByEmail(GetUserByEmailRequest getUserByEmailRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<GetUsersResponse> GetUsers()
        {
            throw new NotImplementedException();
        }

        public async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest updateUserRequest)
        {
            throw new NotImplementedException();
        }
    }
}