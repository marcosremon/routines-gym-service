using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RoutinesGymService.Domain.Model.Entities;
using RoutinesGymService.Transversal.Security.Utils;

namespace RoutinesGymService.Infraestructure.Persistence.Context
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordUtils _passwordUtils;

        public DatabaseSeeder(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _passwordUtils = new PasswordUtils(configuration);
        }

        public async Task SeedAdminUserAsync()
        {
            User? existingAdmin = await _context.Users.FirstOrDefaultAsync(u => u.Email == "admin");
            if (existingAdmin != null)
            {
                Console.WriteLine("Usuario admin ya existe, omitiendo creación...");
                return;
            }

            User adminUser = new User
            {
                Dni = "00000000A",
                Username = "Administrador",
                Surname = "Sistema",
                Email = "admin",
                FriendCode = "@boss",
                Password = _passwordUtils.PasswordEncoder("1234"),
                Role = 1,
                RoleString = "admin",
                InscriptionDate = DateTime.UtcNow,
                SerialNumber = Guid.NewGuid().ToString()
            };

            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();

            Console.WriteLine("Usuario admin creado correctamente.");
        }
    }
}
