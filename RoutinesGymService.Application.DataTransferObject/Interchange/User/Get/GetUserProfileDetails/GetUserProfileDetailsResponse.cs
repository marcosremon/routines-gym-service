using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.User.Get.GetUserProfileDetails
{
    public class GetUserProfileDetailsResponse : BaseResponse
    {
        public string? Username { get; set; }
        public DateTime? InscriptionDate { get; set; }
        public int? RoutineCount { get; set; }
    }
}