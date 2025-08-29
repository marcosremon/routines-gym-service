using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Transversal.JsonInterchange.Stat.GetDailyStepsInfo
{
    public class GetDailyStepsInfoResponseJson : BaseResponseJson
    {
        public int? DailyStepsGoal { get; set; }
        public int? DailySteps { get; set; }
    }
}