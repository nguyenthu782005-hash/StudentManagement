using System.Net;
using System.Net.Mail;

namespace ConnectDB.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpHost = _config["Email:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_config["Email:SmtpPort"] ?? "587");
            var senderEmail = _config["Email:SenderEmail"];
            var senderPassword = _config["Email:SenderPassword"];
            var senderName = _config["Email:SenderName"] ?? "THU STORE";

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            var message = new MailMessage()
            {
                From = new MailAddress(senderEmail!, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            await client.SendMailAsync(message);
        }
    }
}
