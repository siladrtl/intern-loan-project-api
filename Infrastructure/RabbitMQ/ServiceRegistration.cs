using internLoanProjectAPI.Application.Abstractions.Messaging;
using internLoanProjectAPI.RabbitMQ.Email;
using internLoanProjectAPI.RabbitMQ;
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
            //RabbitMQPublisher
            services.AddScoped<IMessagePublisher, MessagePublisher>();
            
            // Mail DI Kaydi
            services.AddHostedService<EmailNotificationConsumer>();
            services.AddSingleton<IEmailService, EmailService>();

            services.AddScoped<IEmailTemplateService, EmailTemplateService>();  
            services.AddScoped<ILoanPaymentPlanPdfService,  LoanPaymentPlanPdfService>();
        }
    }
}
