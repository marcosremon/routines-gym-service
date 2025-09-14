using Microsoft.Extensions.Configuration;
using RoutinesGymService.Transversal.Security;
using Npgsql;

namespace RoutinesGymService.Infraestructure.Persistence.Context
{
    public class DatabaseSeeder
    {
        private readonly IConfiguration _configuration;
        private readonly PasswordUtils _passwordUtils;

        public DatabaseSeeder(IConfiguration configuration)
        {
            _configuration = configuration;
            _passwordUtils = new PasswordUtils(configuration);
        }

        public async Task SeedAdminUserAsync()
        {
            string? connectionString = _configuration.GetConnectionString("PostgreSQLConnection");

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string checkQuery = "SELECT COUNT(*) FROM users WHERE email = 'admin'";
                using (var checkCommand = new NpgsqlCommand(checkQuery, connection))
                {
                    long? count = (long?) await checkCommand.ExecuteScalarAsync();
                    if (count > 0)
                    {
                        Console.WriteLine("Usuario admin ya existe, omitiendo creación...");
                        return;
                    }
                }

                string insertQuery = @"
                    INSERT INTO users (
                        dni, username, surname, email, friend_code, 
                        password, role, role_string, inscription_date
                    )
                    VALUES (
                        @dni, @username, @surname, @email, @friend_code,
                        @password, @role, @role_string, @inscription_date
                    )";

                byte[] encryptedPassword = _passwordUtils.PasswordEncoder("1234");

                using (var command = new NpgsqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@dni", "00000000A");
                    command.Parameters.AddWithValue("@username", "Administrador");
                    command.Parameters.AddWithValue("@surname", "Sistema");
                    command.Parameters.AddWithValue("@email", "admin");
                    command.Parameters.AddWithValue("@friend_code", $"@boss");
                    command.Parameters.AddWithValue("@password", encryptedPassword);
                    command.Parameters.AddWithValue("@role", 1);
                    command.Parameters.AddWithValue("@role_string", "admin");
                    command.Parameters.AddWithValue("@inscription_date", DateTime.UtcNow);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}