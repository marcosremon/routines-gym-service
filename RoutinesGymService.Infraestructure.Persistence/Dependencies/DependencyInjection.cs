using Microsoft.Extensions.DependencyInjection;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Application.Interface.Repository;
using RoutinesGymService.Application.UseCase;
using RoutinesGymService.Infraestructure.Persistence.Repositories;
using RoutinesGymService.Transversal.Common.Utils;
using RoutinesGymService.Transversal.Security.Filters;
using RoutinesGymService.Transversal.Security.Utils;

namespace RoutinesGymService.Infraestructure.Persistence.Dependencies
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Application
            services.AddScoped<IUserApplication, UserApplication>();
            services.AddScoped<IAdminApplication, AdminApplication>();
            services.AddScoped<IRoutineApplication, RoutineApplication>();
            services.AddScoped<ISplitDayApplication, SplitDayApplication>();
            services.AddScoped<IExerciseApplication, ExerciseApplication>();
            services.AddScoped<IAuthApplication, AuthApplication>();
            services.AddScoped<IFriendApplication, FriendApplication>();
            services.AddScoped<IStepApplication, StepApplication>();

            // Repository
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IRoutineRepository, RoutineRepository>();
            services.AddScoped<ISplitDayRepository, SplitDayRepository>();
            services.AddScoped<IExerciseRepository, ExerciseRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IFriendRepository, FriendRepository>();
            services.AddScoped<IStepRepository, StepRepository>();

            // Filtros 
            services.AddScoped<JwtValidationFilter>();
            services.AddScoped<ResourceAuthorizationFilter>(provider =>
            {
                string[] allowedRoles = { "USER", "ADMIN" };
                return new ResourceAuthorizationFilter(allowedRoles);
            });

            services.AddScoped<AdminAuthorizationFilter>();

            // Other services
            services.AddScoped<GenericUtils>();
            services.AddScoped<PasswordUtils>();

            return services;
        }
    }
}