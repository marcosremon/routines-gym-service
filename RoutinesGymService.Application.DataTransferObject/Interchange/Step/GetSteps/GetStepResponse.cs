using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Step.GetStats
{
    public class GetStepResponse : BaseResponse
    {
        public List<RoutinesGymApp.Domain.Entities.Step> Stats { get; set; } = new List<RoutinesGymApp.Domain.Entities.Step>();
    }
}
