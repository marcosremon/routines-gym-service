using System.Net.Mail;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace RoutinesGymService.Transversal.Mailing 
{ 
    public static class MailUtils
    {
        private static IConfiguration? _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static void SendEmail(string recipientName, string recipientEmail, string newPassword)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            string? smtpHost = emailSettings["SmtpHost"];
            int smtpPort = int.Parse(emailSettings["SmtpPort"]);
            string? senderEmail = emailSettings["SenderEmail"];
            string? appPassword = emailSettings["AppPassword"];
            string? senderName = emailSettings["SenderName"];

            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress(recipientName, recipientEmail));
            message.Subject = "Tu nueva contraseña";

            message.Body = new TextPart("plain")
            {
                Text = $"Hola {recipientName},\n\nTu nueva contraseña es: {newPassword}\n\n" +
                       "Por seguridad, te recomendamos cambiar esta contraseña después de iniciar sesión."
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Timeout = 10000;
                client.Connect(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                client.Authenticate(senderEmail, appPassword);
                client.Send(message);
                client.Disconnect(true);
            }
        }

        public static void SendEmailAfterCreatedAccountByGoogle(string recipientName, string recipientEmail)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            string? smtpHost = emailSettings["SmtpHost"];
            int smtpPort = int.Parse(emailSettings["SmtpPort"]);
            string? senderEmail = emailSettings["SenderEmail"];
            string? appPassword = emailSettings["AppPassword"];
            string? senderName = emailSettings["SenderName"];

            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress(recipientName, recipientEmail));
            message.Subject = "Account Information";

            message.Body = new TextPart("plain")
            {
                Text = $"Hello {recipientName},\n\nAfter creating your account by google, your password is the same as your email, you should change it for your security"
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Timeout = 10000;
                client.Connect(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                client.Authenticate(senderEmail, appPassword);
                client.Send(message);
                client.Disconnect(true);
            }
        }

        public static bool IsEmailValid(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    } 
}