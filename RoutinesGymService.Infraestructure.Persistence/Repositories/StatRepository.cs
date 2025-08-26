using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using RoutinesGymApp.Domain.Entities;
using RoutinesGymService.Application.DataTransferObject.Interchange.Stat.GetStats;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Infraestructure.Persistence.Context;
using RoutinesGymService.Transversal.Common;
using RoutinesGymService.Transversal.JsonInterchange.Stat.SaveDailySteps;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class StatRepository : IStatRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly CacheService _cache;
        private readonly GenericUtils _genericUtils;
        private readonly string _statPrefix;
        private readonly int _expiryMinutes;

        public StatRepository(ApplicationDbContext context, CacheService cache, GenericUtils genericUtils, IConfiguration configuration)
        {
            _context = context;
            _cache = cache;
            _statPrefix = configuration["CacheSettings:StatPrefix"]!;
            _genericUtils = genericUtils;
            _expiryMinutes = int.TryParse(configuration["CacheSettings:CacheExpiryMinutes"], out var m) ? m : 60;
        }

        public async Task<GetStatsResponse> GetStats(GetStatRequest getStatRequest)
        {
            GetStatsResponse getStatsResponse = new GetStatsResponse();
            try
            {
                string cacheKey = $"{_statPrefix}_GetStats_{getStatRequest.UserEmail}";
                
                List<Stat>? cachedStats = _cache.Get<List<Stat>>(cacheKey);
                if (cachedStats != null && cachedStats.Any())
                {
                    getStatsResponse.IsSuccess = true;
                    getStatsResponse.Message = "Stats retrieved successfully from cache.";
                    getStatsResponse.Stats = cachedStats;
                }
                else
                {
                    User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == getStatRequest.UserEmail);
                    if (user == null)
                    {
                        getStatsResponse.IsSuccess = false;
                        getStatsResponse.Message = "User not found";
                    }
                    else
                    {
                        List<Stat> stats = await _context.Stats
                            .Where(s => s.UserId == user.UserId)
                            .ToListAsync();

                        if (!stats.Any())
                        {
                            getStatsResponse.IsSuccess = false;
                            getStatsResponse.Message = "No stats found.";
                        }
                        else
                        {
                            getStatsResponse.IsSuccess = true;
                            getStatsResponse.Message = "Stats retrieved successfully.";
                            getStatsResponse.Stats = stats;

                            _cache.Set(cacheKey, stats, TimeSpan.FromMinutes(_expiryMinutes));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                getStatsResponse.Message = $"unexpected error on StatRepository -> GetStats: {ex.Message}";
                getStatsResponse.IsSuccess = false;
            }
            return getStatsResponse;
        }

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
                    Stat stat = new Stat
                    {
                        Steps = saveDailyStepsRequest.Steps,
                        UserId = user.UserId,
                        Date = DateTime.UtcNow.AddDays(-1),
                    };

                    _genericUtils.ClearCache(_statPrefix);

                    _context.Stats.Add(stat);
                    await _context.SaveChangesAsync();

                    saveDailyStepsResponse.IsSuccess = true;
                    saveDailyStepsResponse.Message = "save stats successfuyly";
                }
            } 
            catch (Exception ex)
            {
                saveDailyStepsResponse.Message = $"unexpected error on StatRepository -> SaveDailySteps: {ex.Message}";
                saveDailyStepsResponse.IsSuccess = false;
            }

            return saveDailyStepsResponse;
        }
    }
}