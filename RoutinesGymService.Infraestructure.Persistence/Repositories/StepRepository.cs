using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RoutinesGymApp.Domain.Entities;
using RoutinesGymService.Application.DataTransferObject.Interchange.Step.GetDailyStepsInfo;
using RoutinesGymService.Application.DataTransferObject.Interchange.Step.GetStats;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Infraestructure.Persistence.Context;
using RoutinesGymService.Transversal.Common.Utils;
using RoutinesGymService.Transversal.JsonInterchange.Step.SaveDailySteps;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class StepRepository : IStepRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly CacheUtils _cacheUtils;
        private readonly GenericUtils _genericUtils;
        private readonly string _stepPrefix;
        private readonly int _expiryMinutes;

        public StepRepository(ApplicationDbContext context, CacheUtils cacheUtils, GenericUtils genericUtils, IConfiguration configuration)
        {
            _context = context;
            _cacheUtils = cacheUtils;
            _stepPrefix = configuration["CacheSettings:StepPrefix"]!;
            _genericUtils = genericUtils;
            _expiryMinutes = int.TryParse(configuration["CacheSettings:CacheExpiryMinutes"], out var m) ? m : 60;
        }

        #region Get daily steps info
        public async Task<GetDailyStepsInfoResponse> GetDailyStepsInfo(GetDailyStepsInfoRequest getDailyStepsInfoRequest)
        {
            GetDailyStepsInfoResponse getDailyStepsInfoResponse = new GetDailyStepsInfoResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == getDailyStepsInfoRequest.UserEmail);
                if (user == null)
                {
                    getDailyStepsInfoResponse.IsSuccess = false;
                    getDailyStepsInfoResponse.Message = "user not found";
                }
                else
                {
                    DateTime day = getDailyStepsInfoRequest.Day.Date;
                    Step? step = await _context.Steps.FirstOrDefaultAsync(st => st.Date.Date == day && 
                        st.Steps == getDailyStepsInfoRequest.DailySteps &&
                        st.UserId == user.UserId);
                    if (step == null)
                    {
                        getDailyStepsInfoResponse.IsSuccess = false;
                        getDailyStepsInfoResponse.Message = "steps stats not found";
                    }
                    else
                    {
                        getDailyStepsInfoResponse.IsSuccess = true;
                        getDailyStepsInfoResponse.Message = "steps stats found successfully";
                        getDailyStepsInfoResponse.DailyStepsGoal = step.DailyStepsGoal;
                        getDailyStepsInfoResponse.DailySteps = step.Steps;
                    }
                }
            }
            catch (Exception ex)
            {
                getDailyStepsInfoResponse.Message = $"unexpected error on StatRepository -> GetDailyStepsInfo: {ex.Message}";
                getDailyStepsInfoResponse.IsSuccess = false;
            }

            return getDailyStepsInfoResponse;
        }
        #endregion

        #region Get steps
        public async Task<GetStepResponse> GetStpes(GetStepRequest getStepRequest)
        {
            GetStepResponse getStepResponse = new GetStepResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == getStepRequest.UserEmail);
                if (user == null)
                {
                    getStepResponse.IsSuccess = false;
                    getStepResponse.Message = "User not found";
                }
                else
                {
                    List<Step> steps = await _context.Steps
                        .Where(s => s.UserId == user.UserId)
                        .OrderBy(s => s.Date)
                        .ToListAsync();
                    if (!steps.Any())
                    {
                        getStepResponse.IsSuccess = false;
                        getStepResponse.Message = "No steps found.";
                    }
                    else
                    {
                        getStepResponse.IsSuccess = true;
                        getStepResponse.Message = "Stats retrieved successfully.";
                        getStepResponse.Stats = steps;
                        _cacheUtils.Set(cacheKey, steps, TimeSpan.FromMinutes(_expiryMinutes));
                    }
                }
            }
            catch (Exception ex)
            {
                getStepResponse.Message = $"unexpected error on StatRepository -> GetStats: {ex.Message}";
                getStepResponse.IsSuccess = false;
            }
            return getStepResponse;
        }
        #endregion

        #region Save daily steps
        public async Task<SaveDailyStepsResponse> SaveDailySteps(SaveDailyStepsRequest saveDailyStepsRequest)
        {
            SaveDailyStepsResponse saveDailyStepsResponse = new SaveDailyStepsResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == saveDailyStepsRequest.UserEmail);
                if (user == null)
                {
                    saveDailyStepsResponse.Message = $"user not found";
                    saveDailyStepsResponse.IsSuccess = false;
                }
                else
                {
                    Step step = new Step
                    {
                        Steps = saveDailyStepsRequest.Steps,
                        UserId = user.UserId,
                        DailyStepsGoal = saveDailyStepsRequest.DailyStepsGoal,
                        Date = DateTime.UtcNow.AddDays(-1),
                    };

                    _genericUtils.ClearCache(_stepPrefix);

                    _context.Steps.Add(step);
                    await _context.SaveChangesAsync();

                    saveDailyStepsResponse.IsSuccess = true;
                    saveDailyStepsResponse.Message = "save steps successfuyly";
                }
            }
            catch (Exception ex)
            {
                saveDailyStepsResponse.Message = $"unexpected error on StatRepository -> SaveDailySteps: {ex.Message}";
                saveDailyStepsResponse.IsSuccess = false;
            }

            return saveDailyStepsResponse;
        }
        #endregion
    }
}