using internLoanProjectAPI.Application.Abstractions.Messaging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.RabbitMQ.Email
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(string to, string subject, string body)

        {
            var email = _configuration["EmailSettings:Email"];
            var password = _configuration["EmailSettings:Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Email ayarları bulunamadı.");
            }


            using var smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                    Credentials = new NetworkCredential(email, password),
                    EnableSsl = true
            };


            using var mailMessage = new MailMessage
            {
                    From = new MailAddress(email),
                    Subject = subject,
                    Body = body, 
                    IsBodyHtml = true    
            };
           
            mailMessage.To.Add(to);

            await smtpClient
                .SendMailAsync(mailMessage);
                   
        }
    }
}


