using KodiksCase.Infrastructure.Managers;
using KodiksCase.Shared.DTOs.RabbitMq;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace KodiksCase.Infrastructure.Messaging
{
    public class RabbitMqMessagePublisher : IMessagePublisher
    {
        private readonly IRabbitMqConnectionManager connectionManager;
        private readonly IChannel channel;
        private readonly string queueName;

        public RabbitMqMessagePublisher(IRabbitMqConnectionManager connectionManager, string queueName)
        {
            this.connectionManager = connectionManager;
            this.queueName = queueName;
            this.channel = this.connectionManager.CreateChannel().GetAwaiter().GetResult();

            channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }


        public async Task PublishOrderPlacedAsync(RabbitMqOrderDto order)
        {
            var json = JsonSerializer.Serialize(order);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: queueName,
                mandatory: true,
                basicProperties: new BasicProperties { Persistent = true },
                body: body);
        }
    }
}
