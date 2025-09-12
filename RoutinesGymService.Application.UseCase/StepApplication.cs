using RoutinesGymService.Application.DataTransferObject.Interchange.Step.GetDailyStepsInfo;
using RoutinesGymService.Application.DataTransferObject.Interchange.Step.GetStats;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Transversal.JsonInterchange.Step.SaveDailySteps;

namespace RoutinesGymService.Application.UseCase
{
    public class StepApplication : IStepApplication
    {
        private readonly IStepRepository _stepRepository;

        public StepApplication(IStepRepository stepRepository)
        {
            _stepRepository = stepRepository;
        }

        public async Task<GetDailyStepsInfoResponse> GetDailyStepsInfo(GetDailyStepsInfoRequest getDailyStepsInfoRequest)
        {
            return await _stepRepository.GetDailyStepsInfo(getDailyStepsInfoRequest);
        }

        public async Task<GetStepResponse> GetSteps(GetStepRequest getStepRequest)
        {
            return await _stepRepository.GetStpes(getStepRequest);
        }

        public async Task<SaveDailyStepsResponse> SaveDailySteps(SaveDailyStepsRequest saveDailyStepsRequest)
        {
            return await _stepRepository.SaveDailySteps(saveDailyStepsRequest);
        }
    }
}