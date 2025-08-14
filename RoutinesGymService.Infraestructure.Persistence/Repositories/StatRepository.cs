using Microsoft.EntityFrameworkCore;
using RoutinesGymApp.Domain.Entities;
using RoutinesGymService.Application.DataTransferObject.Interchange.Stat.GetStats;
using RoutinesGymService.Application.DataTransferObject.Interchange.User.Create.CreateUser;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Domain.Model.Entities;
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

        public async Task<GetStatsResponse> GetStats(GetStatRequest getStatRequest)
        {
            GetStatsResponse getStatsResponse = new GetStatsResponse();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == getStatRequest.UserEmail);
                if (user == null)
                {
                    getStatsResponse.IsSuccess = false;
                    getStatsResponse.Message = "User not found.";
                }
                else
                {
                    List<Stat> stats = await _context.Stats.Where(s => s.UserId == user.UserId).ToListAsync();
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