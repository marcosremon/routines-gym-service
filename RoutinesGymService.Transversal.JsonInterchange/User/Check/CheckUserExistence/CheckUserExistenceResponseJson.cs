using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.User.Check.CheckUserExistence
{
    public class CheckUserExistenceResponseJson : BaseResponseJson
    {
        public bool UserExists { get; set; }
    }
}