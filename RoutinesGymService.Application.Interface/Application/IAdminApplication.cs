using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.ChangeUserPassword;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.ChangeUserRole;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetBlacklistedUsers;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetIntegralUserInfo;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetIntegralUsers;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetUsers;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.GetUsersByRole;
using RoutinesGymService.Application.DataTransferObject.Interchange.Admin.RemoveUserFromBlackList;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.AddUserToBlackList;

namespace RoutinesGymService.Application.Interface.Application
{
    public interface IAdminApplication
    {
        Task<AddUserToBlackListResponse> AddUserToBlackList(AddUserToBlackListRequest addUserToBlackListRequest);
        Task<ChangeUserPasswordResponse> ChangeUserPassword(ChangeUserPasswordRequest changeUserPasswordRequest);
        Task<ChangeUserRoleResponse> ChangeUserRole(ChangeUserRoleRequest changeUserRoleRequest);
        Task<GetBlacklistedUsersResponse> GetBlacklistedUsers(GetBlacklistedUsersRequest getBlacklistedUsersRequest);
        Task<GetIntegralUserInfoResponse> GetIntegralUserInfo(GetIntegralUserInfoRequest getIntegralUserInfoRequest);
        Task<GetIntegralUsersResponse> GetIntegralUsers(GetIntegralUsersRequest getIntegralUsersRequest);
        Task<GetUsersResponse> GetUsers();
        Task<GetUsersByRoleResponse> GetUsersByRole(GetUsersByRoleRequest getUsersByRoleRequest);
        Task<RemoveUserFromBlackListResponse> RemoveUserFromBlackList(RemoveUserFromBlackListRequest removeUserFromBlackListRequest);
    }
}