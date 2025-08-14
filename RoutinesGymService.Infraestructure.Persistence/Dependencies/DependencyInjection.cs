using Microsoft.Extensions.DependencyInjection;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Application.UseCase;
using RoutinesGymService.Infraestructure.Persistence.Repositories;

namespace RoutinesGymService.Infraestructure.Persistence.Dependencies
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Application
            services.AddScoped<IUserApplication, UserApplication>();
            services.AddScoped<IRoutineApplication, RoutineApplication>();
            services.AddScoped<ISplitDayApplication, SplitDayApplication>();
            services.AddScoped<IExerciseApplication, ExerciseApplication>();
            services.AddScoped<IAuthApplication, AuthApplication>();
            services.AddScoped<IFriendApplication, FriendApplication>();
            services.AddScoped<IStatApplication, StatApplication>();

            // Repository
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoutineRepository, RoutineRepository>();
            services.AddScoped<ISplitDayRepository, SplitDayRepository>();
            services.AddScoped<IExerciseRepository, ExerciseRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IFriendRepository, FriendRepository>();
            services.AddScoped<IStatRepository, StatRepository>();

            return services;
        }
    }
}