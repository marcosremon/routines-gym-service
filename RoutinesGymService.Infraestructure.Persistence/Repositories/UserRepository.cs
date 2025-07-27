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

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        public Task<ChangePasswordWithPasswordAndEmailResponse> ChangePasswordWithPasswordAndEmail(ChangePasswordWithPasswordAndEmailRequest changePasswordWithPasswordAndEmailRequest)
        {
            throw new NotImplementedException();
        }

        public Task<CreateGoogleUserResponse> CreateGoogleUser(CreateGenericUserRequest createGenericUserRequest)
        {
            throw new NotImplementedException();
        }

        public Task<CreateNewPasswordResponse> CreateNewPassword(CreateNewPasswordRequest createNewPasswordRequest)
        {
            throw new NotImplementedException();
        }

        public Task<CreateUserResponse> CreateUser(CreateGenericUserRequest createGenericUserRequest)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteUserResponse> DeleteUser(DeleteUserRequest deleteUserRequest)
        {
            throw new NotImplementedException();
        }

        public Task<GetUserByEmailResponse> GetUserByEmail(GetUserByEmailRequest getUserByEmailRequest)
        {
            throw new NotImplementedException();
        }

        public Task<GetUsersResponse> GetUsers()
        {
            throw new NotImplementedException();
        }

        public Task<UpdateUserResponse> UpdateUser(UpdateUserRequest updateUserRequest)
        {
            throw new NotImplementedException();
        }
    }
}