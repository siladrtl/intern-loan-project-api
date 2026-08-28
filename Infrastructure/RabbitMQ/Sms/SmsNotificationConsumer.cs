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

namespace internLoanProjectAPI.RabbitMQ.Sms
{
    public class SmsNotificationConsumer : BackgroundService

    {
        private IConnection? _connection;
        private IChannel? _channel;
        private ISmsService _smsService;

        public SmsNotificationConsumer(ISmsService smsService)
        {
            _smsService = smsService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            _connection = await factory.CreateConnectionAsync();

            _channel = await _connection.CreateChannelAsync();


            // Dinlenilecek queue
            await _channel.QueueDeclareAsync(
                queue: "sms-notification-queue",
                durable: true,
                exclusive: false,
                autoDelete: false
            );


            var consumer = new AsyncEventingBasicConsumer(_channel);


            consumer.ReceivedAsync +=
                async (sender, args) =>
                {
                    var body = args.Body.ToArray();

                    var json = Encoding.UTF8.GetString(body);


                    var message = JsonSerializer.Deserialize<LoanApplicationSmsMessage>(json);
                          
                    if (message != null)
                    {
                        await _smsService.SendAsync(
                            message.PhoneNumber,
                            message.Message
                        );
                    }

                    // Mesaj basariyle islendi
                    await _channel.BasicAckAsync(
                        deliveryTag:
                            args.DeliveryTag,

                        multiple: false
                    );
                };


            await _channel.BasicConsumeAsync(
                queue: "sms-notification-queue",

                autoAck: false,

                consumer: consumer
            );


            // BackgroundService calismaya devam etsin
            await Task.Delay(
                Timeout.Infinite,
                stoppingToken
            );
        }


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
