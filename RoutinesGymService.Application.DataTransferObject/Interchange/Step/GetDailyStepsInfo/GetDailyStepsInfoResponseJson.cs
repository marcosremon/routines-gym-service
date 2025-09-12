using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Step.GetDailyStepsInfo
{
    public class GetDailyStepsInfoResponse : BaseResponse
    {
        public int? DailyStepsGoal { get; set; }
        public int? DailySteps { get; set; }
    }
}