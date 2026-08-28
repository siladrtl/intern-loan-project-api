using internLoanProjectAPI.Application.Abstractions.Messaging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace internLoanProjectAPI.RabbitMQ
{
    public class MessagePublisher : IMessagePublisher
    {
        public async Task PublishAsync<T>(T message, string queueName)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            await using var connection = await factory.CreateConnectionAsync();


            await using var channel = await connection.CreateChannelAsync();


            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );


            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
                
            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: queueName,
                body: body
            );
        }
    }
}
