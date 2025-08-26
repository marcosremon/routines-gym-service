using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.User.Get.GetUserProfileDetails
{
    public class GetUserProfileDetailsResponseJson : BaseResponseJson
    {
        public string? Username { get; set; }
        public DateTime? InscriptionDate { get; set; }
        public int? RoutineCount { get; set; }
    }
}