
using RoutinesGymService.Application.DataTransferObject.Interchange.Step.GetDailyStepsInfo;
using RoutinesGymService.Application.DataTransferObject.Interchange.Step.GetStats;
using RoutinesGymService.Transversal.JsonInterchange.Step.SaveDailySteps;

namespace RoutinesGymService.Application.Interface.Repository
{
    public interface IStepRepository
    {
        Task<GetDailyStepsInfoResponse> GetDailyStepsInfo(GetDailyStepsInfoRequest getDailyStepsInfoRequest);
        Task<GetStepResponse> GetStpes(GetStepRequest getStepRequest);
        Task<SaveDailyStepsResponse> SaveDailySteps(SaveDailyStepsRequest saveDailyStepsRequest);
    }
}