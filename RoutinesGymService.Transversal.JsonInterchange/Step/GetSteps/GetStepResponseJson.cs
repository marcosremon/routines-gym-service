using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Step.GetStats
{
    public class GetStepResponseJson : BaseResponseJson
    {
        public List<RoutinesGymApp.Domain.Entities.Step> Steps { get; set; } = new List<RoutinesGymApp.Domain.Entities.Step>();
    }
}