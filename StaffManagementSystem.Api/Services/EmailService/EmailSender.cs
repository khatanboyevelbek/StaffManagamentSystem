using System.Net;
using System.Net.Mail;
using StaffManagementSystem.Api.Domain.DTOs;
using StaffManagementSystem.Api.Domain.Entities;

namespace StaffManagementSystem.Api.Services.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration configuration;

        public EmailSender(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void SendEmail(string userEmail, string userPassword, string actionMessage)
        {
            var emailSettings = this.configuration.GetSection("EmailSettings");
            string smtpServer = emailSettings["SmtpServer"];
            int smtpPort = int.Parse(emailSettings["SmtpPort"]);
            bool useSsl = bool.Parse(emailSettings["UseSsl"]);
            string username = emailSettings["Username"];
            string password = emailSettings["Password"];

            var smtpClient = new SmtpClient
            {
                Host = smtpServer,
                Port = smtpPort,
                EnableSsl = useSsl,
                Credentials = new NetworkCredential(
                    username,
                    password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(username),
                Subject = "Foydalanuvchi ma'lumotlari",
                Body = actionMessage,
                IsBodyHtml = true
            };

            mailMessage.To.Add(userEmail);

            smtpClient.Send(mailMessage);
        }
    }
}
