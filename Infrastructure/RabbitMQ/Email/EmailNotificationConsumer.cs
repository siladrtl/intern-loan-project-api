using internLoanProjectAPI.Application.Abstractions.Messaging;
using internLoanProjectAPI.Application.Messages;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace internLoanProjectAPI.RabbitMQ
{
    public class EmailNotificationConsumer : BackgroundService
    {
        private readonly IEmailService _emailService;

        private IConnection? _connection;
        private IChannel? _channel;

        public EmailNotificationConsumer(IEmailService emailService)
        {
            _emailService = emailService;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };


            _connection =
                await factory.CreateConnectionAsync();


            _channel =
                await _connection.CreateChannelAsync();


            // Dinlenecek queue
            await _channel.QueueDeclareAsync(
                queue: "email-notification-queue",
                durable: true,
                exclusive: false,
                autoDelete: false
            );


            var consumer =
                new AsyncEventingBasicConsumer(
                    _channel
                );


            consumer.ReceivedAsync +=
                async (sender, args) =>
                {
                    var body =
                        args.Body.ToArray();


                    var json =
                        Encoding.UTF8.GetString(body);


                    var message =
                        JsonSerializer.Deserialize<
                            LoanApplicationEmailMessage
                        >(json);


                    if (message != null)
                    {
                        try
                        {
                            Console.WriteLine(
                                $"Mail gönderiliyor -> {message.Email}"
                            );

                            await _emailService.SendAsync(
                                message.Email,
                                message.Subject,
                                message.Message
                            );

                            Console.WriteLine(
                                "Mail başarıyla gönderildi."
                            );


                            await _channel.BasicAckAsync(
                                deliveryTag: args.DeliveryTag,
                                multiple: false
                            );
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(
                                "EMAIL GÖNDERME HATASI:"
                            );

                            Console.WriteLine(
                                ex.Message
                            );

                            Console.WriteLine(
                                ex.ToString()
                            );
                        }
                    }


                    // Mesaj başarıyla işlendi
                    await _channel.BasicAckAsync(
                        deliveryTag: args.DeliveryTag,
                        multiple: false
                    );
                };


            await _channel.BasicConsumeAsync(
                queue: "email-notification-queue",
                autoAck: false,
                consumer: consumer
            );


            // Consumer arka planda çalışmaya devam etsin
            await Task.Delay(
                Timeout.Infinite,
                stoppingToken
            );
        }


        public override async Task StopAsync(
            CancellationToken cancellationToken)
        {
            if (_channel != null)
            {
                await _channel.DisposeAsync();
            }


            if (_connection != null)
            {
                await _connection.DisposeAsync();
            }


            await base.StopAsync(
                cancellationToken
            );
        }
    }
}





