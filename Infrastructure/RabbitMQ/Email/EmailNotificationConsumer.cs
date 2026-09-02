using internLoanProject.Domain.Entities;
using internLoanProject.Domain.Entities.Enums;
using internLoanProjectAPI.Application.Abstractions.Messaging;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace internLoanProjectAPI.RabbitMQ
{
    public class EmailNotificationConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection? _connection;
        private IChannel? _channel;

       
        // CONSTRUCTOR
        public EmailNotificationConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        
        // CONSUMER
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
         
            // RABBITMQ BAĞLANTISI
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

         
            // QUEUE
            await _channel.QueueDeclareAsync(
                queue: "email-notification-queue",
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);

           
            // MESAJ GELDİĞİNDE
            consumer.ReceivedAsync += async (sender, args) =>
            {
                try
                {
                    
                    // RABBITMQ MESAJINI OKU
                    var body = args.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<LoanApplicationEmailMessage>(json);

                    if (message == null)
                    {
                        await _channel.BasicNackAsync(
                            deliveryTag: args.DeliveryTag,
                            multiple: false,
                            requeue: false
                        );
                        return;
                    }

                    Console.WriteLine($"Mail hazırlanıyor -> Başvuru ID: {message.ApplicationId}");

                    
                    // SCOPE
                   await using var scope = _scopeFactory.CreateAsyncScope();

                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var emailTemplateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();
                    var pdfService = scope.ServiceProvider.GetRequiredService<ILoanPaymentPlanPdfService>();

                 
                    // BAŞVURU DETAYLARINI GETİR
                    var application = await unitOfWork
                        .GetReadRepository<LoanApplication>()
                        .GetAll(false)
                        .Include(x => x.Customer)
                        .Include(x => x.LoanProduct)
                            .ThenInclude(x => x.Bank)
                        .Include(x => x.LoanCalculation)
                            .ThenInclude(x => x.PaymentPlans)
                        .FirstOrDefaultAsync(x => x.Id == message.ApplicationId, stoppingToken);

                    if (application == null)
                    {
                        throw new Exception("Mail gönderilecek kredi başvurusu bulunamadı.");
                    }

                    
                    // ONAYLANAN BAŞVURU
                    if (application.Status == LoanApplicationStatus.Approved)
                    {
                        await SendApprovedEmailAsync(application, emailService, emailTemplateService, pdfService);
                    }
                    //
                    // REDDEDİLEN BAŞVURU
                    else if (application.Status == LoanApplicationStatus.Rejected)
                    {
                        await SendRejectedEmailAsync(application, emailService, emailTemplateService);
                    }

                  
                    // BAŞARILI → ACK
                    await _channel.BasicAckAsync(
                        deliveryTag: args.DeliveryTag,
                        multiple: false
                    );

                    Console.WriteLine("Mail başarıyla gönderildi.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("EMAIL GÖNDERME HATASI:");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex);

                  
                    // HATA → NACK
                    if (_channel != null)
                    {
                        await _channel.BasicNackAsync(
                            deliveryTag: args.DeliveryTag,
                            multiple: false,
                            requeue: false
                        );
                    }
                }
            };

            // QUEUE DİNLE
            await _channel.BasicConsumeAsync(
                queue: "email-notification-queue",
                autoAck: false,
                consumer: consumer
            );

            // ARKA PLANDA ÇALIŞMAYA DEVAM ET
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

       
        // ONAY MAILİ GÖNDER
        private static async Task SendApprovedEmailAsync(
            LoanApplication application,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService,
            ILoanPaymentPlanPdfService pdfService)
        {
            var applicationNumber = $"KRD-{application.Id:D6}";

            var emailBody = emailTemplateService.CreateApprovedLoanApplicationEmail(application);
            var pdfBytes = pdfService.Create(application);

            await emailService.SendAsync(
                application.Customer.Email,
                "Kredi Başvurunuz Onaylandı",
                emailBody,
                pdfBytes,
                $"Odeme-Plani-{applicationNumber}.pdf"
            );
        }

     
        // RED MAILİ GÖNDER
        private static async Task SendRejectedEmailAsync(
            LoanApplication application,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService)
        {
            var emailBody = emailTemplateService.CreateRejectedLoanApplicationEmail(application);

            await emailService.SendAsync(
                application.Customer.Email,
                "Kredi Başvurunuz Sonuçlandı",
                emailBody
            );
        }

      
        // CONSUMER DURDUR
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null)
            {
                await _channel.DisposeAsync();
            }

            if (_connection != null)
            {
                await _connection.DisposeAsync();
            }

            await base.StopAsync(cancellationToken);
        }
    }
}