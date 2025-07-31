using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace LadeviVentasApi
{
    public class EmailSender : IEmailSender
    {
        // Our private configuration variables
        private string Host {get;}
        private int Port { get; }
        private bool EnableSsl { get; }
        private string UserName { get; }
        private string Password { get; }

        // Get our parameterized configuration
        public EmailSender(string host, int port, bool enableSsl, string userName, string password)
        {
            Host = host;
            Port = port;
            EnableSsl = enableSsl;
            UserName = userName;
            Password = password;
        }

        // Use our configuration to send the email by using SmtpClient
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(Host, Port)
            {
                Credentials = new NetworkCredential(UserName, Password),
                EnableSsl = EnableSsl
            };
            return client.SendMailAsync(
                new MailMessage(UserName, email, subject, htmlMessage) { IsBodyHtml = true }
            );
        }
    }
}