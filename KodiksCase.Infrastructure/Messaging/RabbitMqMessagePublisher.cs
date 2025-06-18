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
    // Publishes messages to RabbitMQ queue for order-related events.
    public class RabbitMqMessagePublisher : IMessagePublisher
    {
        private readonly IRabbitMqConnectionManager connectionManager;
        private readonly IChannel channel;
        private readonly string queueName;

        /// <summary>
        /// Initializes the publisher with a connection manager and queue name, and declares the queue.
        /// </summary>
        /// <param name="connectionManager">Manages RabbitMQ connections.</param>
        /// <param name="queueName">Name of the RabbitMQ queue to publish messages to.</param>
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

        /// <summary>
        /// Publishes a serialized order placed message asynchronously to the configured RabbitMQ queue.
        /// </summary>
        /// <param name="order">The order DTO to publish as a message.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
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
