using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.User.Get.GetUserProfileDetails
{
    public class GetUserProfileDetailsResponseJson : BaseResponseJson
    {
        public string Username { get; set; } = string.Empty;
        public DateTime InscriptionDate { get; set; } = DateTime.MinValue;
        public int RoutineCount { get; set; } = -1;
    }
}