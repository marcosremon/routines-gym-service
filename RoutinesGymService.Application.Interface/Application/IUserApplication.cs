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

namespace RoutinesGymService.Application.Interface.Application
{
    public interface IUserApplication
    {
        Task<GetUserByEmailResponse> GetUserByEmail(GetUserByEmailRequest getUserByEmailRequest);
        Task<GetUsersResponse> GetUsers();
        Task<UpdateUserResponse> UpdateUser(UpdateUserRequest updateUserRequest);
        Task<DeleteUserResponse> DeleteUser(DeleteUserRequest deleteUserRequest);
        Task<CreateNewPasswordResponse> CreateNewPassword(CreateNewPasswordRequest createNewPasswordRequest);
        Task<ChangePasswordWithPasswordAndEmailResponse> ChangePasswordWithPasswordAndEmail(ChangePasswordWithPasswordAndEmailRequest changePasswordWithPasswordAndEmailRequest);
        Task<CreateGoogleUserResponse> CreateGoogleUser(CreateGenericUserRequest createGenericUserRequest);
        Task<CreateUserResponse> CreateUser(CreateGenericUserRequest createGenericUserRequest);
        Task<GetUserProfileDetailsResponse> GetUserProfileDetails(GetUserProfileDetailsRequest getUserProfileDetailsRequest);
        Task<CheckUserExistenceResponse> CheckUserExistence(CheckUserExistenceRequest checkUserExistenceRequest);
        Task<AddUserToBlackListResponse> AddUserToBlackList(AddUserToBlackListRequest addUserToBlackListRequest);
        Task<GetIntegralUserInfoResponse> GetIntegralUserInfo(GetIntegralUserInfoRequest getIntegralUserInfoRequest);
    }
}