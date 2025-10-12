using RoutinesGymService.Application.DataTransferObject.Interchange.User.Check.CheckUserExistence;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.AddUserToBlackList;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.ChangePasswordWithPasswordAndEmail;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateGenericUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateGoogleUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateNewPassword;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.DeleteUser;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetIntegralUserInfo;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUserByEmail;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUserProfileDetails;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUsers;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.UpdateUser;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Application.Interface.Repository;

namespace RoutinesGymService.Application.UseCase
{
    public class UserApplication : IUserApplication
    {
        private readonly IUserRepository _userRepository;

        public UserApplication(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<AddUserToBlackListResponse> AddUserToBlackList(AddUserToBlackListRequest addUserToBlackListRequest)
        {
            return await _userRepository.AddUserToBlackList(addUserToBlackListRequest);
        }

        public async Task<ChangePasswordWithPasswordAndEmailResponse> ChangePasswordWithPasswordAndEmail(ChangePasswordWithPasswordAndEmailRequest changePasswordWithPasswordAndEmailRequest)
        {
            return await _userRepository.ChangePasswordWithPasswordAndEmail(changePasswordWithPasswordAndEmailRequest);
        }

        public async Task<CheckUserExistenceResponse> CheckUserExistence(CheckUserExistenceRequest checkUserExistenceRequest)
        {
            return await _userRepository.CheckUserExistence(checkUserExistenceRequest);
        }

        public async Task<CreateGoogleUserResponse> CreateGoogleUser(CreateGenericUserRequest createGenericUserRequest)
        {
            return await _userRepository.CreateGoogleUser(createGenericUserRequest);
        }

        public async Task<CreateNewPasswordResponse> CreateNewPassword(CreateNewPasswordRequest createNewPasswordRequest)
        {
            return await _userRepository.CreateNewPassword(createNewPasswordRequest);
        }

        public async Task<CreateUserResponse> CreateUser(CreateGenericUserRequest createGenericUserRequest)
        {
            return await _userRepository.CreateUser(createGenericUserRequest);
        }

        public async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest deleteUserRequest)
        {
            return await _userRepository.DeleteUser(deleteUserRequest);
        }

        public async Task<GetIntegralUserInfoResponse> GetIntegralUserInfo(GetIntegralUserInfoRequest getIntegralUserInfoRequest)
        {
            return await _userRepository.GetIntegralUserInfo(getIntegralUserInfoRequest);
        }

        public async Task<GetUserByEmailResponse> GetUserByEmail(GetUserByEmailRequest getUserByEmailRequest)
        {
            return await _userRepository.GetUserByEmail(getUserByEmailRequest);
        }

        public async Task<GetUserProfileDetailsResponse> GetUserProfileDetails(GetUserProfileDetailsRequest GetUserProfileDetails)
        {
            return await _userRepository.GetUserProfileDetails(GetUserProfileDetails);
        }

        public async Task<GetUsersResponse> GetUsers()
        {
            return await _userRepository.GetUsers();
        }

        public async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest updateUserRequest)
        {
            return await _userRepository.UpdateUser(updateUserRequest);
        }
    }
}