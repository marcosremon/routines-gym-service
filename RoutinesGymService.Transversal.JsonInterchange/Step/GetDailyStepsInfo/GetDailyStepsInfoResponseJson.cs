using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Step.GetDailyStepsInfo
{
    public class GetDailyStepsInfoResponseJson : BaseResponseJson
    {
        public int DailyStepsGoal { get; set; } = -1;
        public int DailySteps { get; set; } = -1;
    }
}