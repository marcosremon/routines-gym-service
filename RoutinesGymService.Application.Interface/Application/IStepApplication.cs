using RoutinesGymService.Application.DataTransferObject.Interchange.Step.GetDailyStepsInfo;
using RoutinesGymService.Application.DataTransferObject.Interchange.Step.GetStats;
using RoutinesGymService.Transversal.JsonInterchange.Step.SaveDailySteps;

namespace RoutinesGymService.Application.Interface.Application
{
    public interface IStepApplication
    {
        Task<GetDailyStepsInfoResponse> GetDailyStepsInfo(GetDailyStepsInfoRequest getDailyStepsInfoRequest);
        Task<GetStepResponse> GetSteps(GetStepRequest getStepsRequest);
        Task<SaveDailyStepsResponse> SaveDailySteps(SaveDailyStepsRequest saveDailyStepsRequest);
    }
}