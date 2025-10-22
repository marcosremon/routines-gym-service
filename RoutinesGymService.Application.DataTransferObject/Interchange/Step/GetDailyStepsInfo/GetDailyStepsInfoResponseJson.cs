using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Step.GetDailyStepsInfo
{
    public class GetDailyStepsInfoResponse : BaseResponse
    {
        public int DailyStepsGoal { get; set; } = -1;
        public int DailySteps { get; set; } = -1;
    }
}