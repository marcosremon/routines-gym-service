using Microsoft.EntityFrameworkCore;
using RoutinesGymService.Domain.Model.Entities;

namespace RoutinesGymService.Infraestructure.Persistence.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserFriend> UserFriends { get; set; }
        public DbSet<Routine> Routines { get; set; }
        public DbSet<SplitDay> SplitDays { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ExerciseProgress> ExerciseProgress { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // No configuration needed - all mappings are handled via attributes
        }
    }
}