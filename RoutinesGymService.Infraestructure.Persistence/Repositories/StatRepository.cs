using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using RoutinesGymApp.Domain.Entities;
using RoutinesGymService.Application.DataTransferObject.Interchange.Stat.GetStats;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Infraestructure.Persistence.Context;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class StatRepository : IStatRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly string _statPrefix;
        private readonly int _expiryMinutes;

        public StatRepository(ApplicationDbContext context, IMemoryCache cache, IConfiguration configuration)
        {
            _context = context;
            _cache = cache;
            _statPrefix = configuration["CacheSettings:StatPrefix"]!;
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

                            MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromMinutes(_expiryMinutes))
                                .SetAbsoluteExpiration(TimeSpan.FromMinutes(_expiryMinutes * 2));

                            _cache.Set(cacheKey, stats, cacheOptions);
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
    }
}