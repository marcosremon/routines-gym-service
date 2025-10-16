using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.ChangeUserRole;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetBlacklistedUsers;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetIntegralUserInfo;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetIntegralUsers;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetUsers;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetUsersByRole;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.RemoveUserFromBlackList;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.AddUserToBlackList;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Application.Interface.Repository;

namespace RoutinesGymService.Application.UseCase
{
    public class AdminApplication : IAdminApplication
    {
        private readonly IAdminRepository _adminRepository;

        public AdminApplication(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<AddUserToBlackListResponse> AddUserToBlackList(AddUserToBlackListRequest addUserToBlackListRequest)
        {
            return await _adminRepository.AddUserToBlackList(addUserToBlackListRequest);
        }

        public async Task<ChangeUserRoleResponse> ChangeUserRole(ChangeUserRoleRequest changeUserRoleRequest)
        {
            return await _adminRepository.ChangeUserRole(changeUserRoleRequest);
        }

        public async Task<GetBlacklistedUsersResponse> GetBlacklistedUsers()
        {
            return await _adminRepository.GetBlacklistedUsers();
        }

        public async Task<GetIntegralUserInfoResponse> GetIntegralUserInfo(GetIntegralUserInfoRequest getIntegralUserInfoRequest)
        {
            return await _adminRepository.GetIntegralUserInfo(getIntegralUserInfoRequest);
        }

        public async Task<GetIntegralUsersResponse> GetIntegralUsers(GetIntegralUsersRequest getIntegralUsersRequest)
        {
            return await _adminRepository.GetIntegralUsers(getIntegralUsersRequest);
        }

        public async Task<GetUsersResponse> GetUsers()
        {
            return await _adminRepository.GetUsers();
        }

        public async Task<GetUsersByRoleResponse> GetUsersByRole(GetUsersByRoleRequest getUsersByRoleRequest)
        {
            return await _adminRepository.GetUsersByRole(getUsersByRoleRequest);
        }

        public async Task<RemoveUserFromBlackListResponse> RemoveUserFromBlackList(RemoveUserFromBlackListRequest removeUserFromBlackListRequest)
        {
            return await _adminRepository.RemoveUserFromBlackList(removeUserFromBlackListRequest);
        }
    }
}