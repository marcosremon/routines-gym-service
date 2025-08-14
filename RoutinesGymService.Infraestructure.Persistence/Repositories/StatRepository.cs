using Microsoft.EntityFrameworkCore;
using RoutinesGymApp.Domain.Entities;
using RoutinesGymService.Application.DataTransferObject.Interchange.Stat.GetStats;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateUser;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Infraestructure.Persistence.Context;

namespace RoutinesGymService.Infraestructure.Persistence.Repositories
{
    public class StatRepository : IStatRepository
    {
        private readonly ApplicationDbContext _context;

        public StatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GetStatsResponse> GetStats()
        {
            GetStatsResponse getStatsResponse = new GetStatsResponse();
            try
            {
                List<Stat> stats = await _context.Stats.ToListAsync();
                if (stats == null || !stats.Any())
                {
                    getStatsResponse.IsSuccess = false;
                    getStatsResponse.Message = "No stats found.";
                }
                else
                {
                    getStatsResponse.IsSuccess = true;
                    getStatsResponse.Message = "Stats retrieved successfully.";
                    getStatsResponse.Stats = stats;
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