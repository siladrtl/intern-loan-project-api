using internLoanProjectAPI.Application.Abstractions.Messaging;
using internLoanProjectAPI.RabbitMQ.Email;
using internLoanProjectAPI.RabbitMQ.Sms;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.RabbitMQ
{
    public static class ServiceRegistration
    {
        public static void AddRabbitMQServices(this IServiceCollection services)
        {
            services.AddScoped<IMessagePublisher, MessagePublisher>();
           
            services.AddHostedService<SmsNotificationConsumer>();
            services.AddSingleton<ISmsService, SmsService>();

            services.AddHostedService<EmailNotificationConsumer>();
            services.AddSingleton<IEmailService, EmailService>();
        }
    }
}
