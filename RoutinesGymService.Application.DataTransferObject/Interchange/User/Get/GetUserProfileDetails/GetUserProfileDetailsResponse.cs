using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUserProfileDetails
{
    public class GetUserProfileDetailsResponse : BaseResponse
    {
        public string Username { get; set; } = string.Empty;
        public DateTime InscriptionDate { get; set; } = DateTime.MinValue;
        public int RoutineCount { get; set; } = -1;
    }
}