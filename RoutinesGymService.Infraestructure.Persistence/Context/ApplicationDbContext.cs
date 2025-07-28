using System.Reflection;
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>(); 

            builder.Entity<SplitDay>()
                .Property(s => s.DayName)
                .HasConversion<string>(); 

            builder.Entity<ExerciseProgress>()
                .Property(e => e.DayName)
                .HasConversion<string>(); 

            builder.Entity<Routine>()
                .HasMany(r => r.SplitDays)
                .WithOne(s => s.Routine)
                .HasForeignKey(s => s.RoutineId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Routine>()
                .HasMany(r => r.Exercises)
                .WithOne(e => e.Routine)
                .HasForeignKey(e => e.RoutineId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Routine>()
                .HasMany(r => r.ProgressEntries)
                .WithOne(p => p.Routine)
                .HasForeignKey(p => p.RoutineId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<SplitDay>()
                .HasMany(s => s.Exercises)
                .WithOne(e => e.SplitDay)
                .HasForeignKey(e => e.SplitDayId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Exercise>()
                .HasMany(e => e.ProgressEntries)
                .WithOne(p => p.Exercise)
                .HasForeignKey(p => p.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<User>()
                .HasMany(u => u.FriendshipsAsUser)
                .WithOne(uf => uf.User)
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<User>()
                .HasMany(u => u.FriendshipsAsFriend)
                .WithOne(uf => uf.Friend)
                .HasForeignKey(uf => uf.FriendId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.EnableSensitiveDataLogging();
            }
        }
    }
}