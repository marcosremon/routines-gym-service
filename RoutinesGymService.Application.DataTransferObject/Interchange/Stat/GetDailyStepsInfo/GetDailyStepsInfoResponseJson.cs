using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.Interchange.Stat.GetDailyStepsInfo
{
    public class GetDailyStepsInfoResponse : BaseResponse
    {
        public int? DailyStepsGoal { get; set; }
        public int? DailySteps { get; set; }
    }
}