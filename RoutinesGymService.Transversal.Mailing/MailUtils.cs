using Microsoft.Extensions.Configuration;

namespace RoutinesGymService.Transversal.Mailing
{
    public class MailUtils
    {
        private static IConfiguration? _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}