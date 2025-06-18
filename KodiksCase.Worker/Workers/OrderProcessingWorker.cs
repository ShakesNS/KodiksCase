using KodiksCase.Infrastructure.Caching;
using KodiksCase.Infrastructure.Managers;
using KodiksCase.Shared.DTOs.RabbitMq;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace KodiksCase.Worker.Workers
{
    public class OrderProcessingWorker : BackgroundService
    {
        private readonly IRabbitMqConnectionManager connectionManager;
        private IChannel channel;
        private readonly string queueName = "order-placed";
        private readonly ILogger<OrderProcessingWorker> logger;
        private readonly IRedisCacheService redisCacheService;

        public OrderProcessingWorker(IRabbitMqConnectionManager connectionManager, IRedisCacheService redisCacheService, ILogger<OrderProcessingWorker> logger)
        {
            this.connectionManager = connectionManager;
            this.redisCacheService = redisCacheService;
            this.logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            channel = connectionManager.CreateChannel().GetAwaiter().GetResult();

            channel.QueueDeclareAsync(queue: queueName,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            // Log that worker and queue are ready with a unique CorrelationId for tracing
            using (LogContext.PushProperty("CorrelationId", Guid.NewGuid().ToString()))
            {
                logger.LogInformation("RabbitMQ channel and queue prepared. Worker started.");
            }
            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(channel);

            // Event handler for received messages
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    // Deserialize the message to order DTO
                    var order = JsonSerializer.Deserialize<RabbitMqOrderDto>(message);

                    // Log reception of new order message with correlation ID
                    using (LogContext.PushProperty("CorrelationId", Guid.NewGuid().ToString()))
                    {
                        logger.LogInformation("A new order message has been received. OrderId: {OrderId}", order.OrderId);
                    }

                    // Simulate processing delay
                    await Task.Delay(1000);

                    // Write processing log into Redis cache
                    await redisCacheService.SetProcessingLogAsync($"processed:{order.OrderId}", $"Processed at {DateTime.Now}");

                    // Log that processing log was written to Redis
                    using (LogContext.PushProperty("CorrelationId", Guid.NewGuid().ToString()))
                    {
                        logger.LogInformation("Order processing log was written to Redis.Order processing log was written to Redis.A new order message was received. OrderId: {OrderId}", order.OrderId);
                    }

                    // Read the processing log from Redis
                    var log = await redisCacheService.GetProcessingLogAsync($"processed:{order.OrderId}");

                    // Log the retrieved Redis processing log
                    using (LogContext.PushProperty("CorrelationId", Guid.NewGuid().ToString()))
                    {
                        logger.LogInformation("Processing log read from Redis: {Log}", log);
                    }

                    // Log successful order processing
                    using (LogContext.PushProperty("CorrelationId", Guid.NewGuid().ToString()))
                    {
                        logger.LogInformation("Order processed successfully. OrderId: {OrderId}", order.OrderId);
                    }

                    // Write logs and status to console for monitoring
                    Console.WriteLine($"Redis log: {log}");
                    Console.WriteLine($"Order processed: {order.OrderId} at {DateTime.Now}");

                    // Acknowledge message as processed to RabbitMQ
                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    // Log any errors during message processing with correlation ID
                    using (LogContext.PushProperty("CorrelationId", Guid.NewGuid().ToString()))
                    {
                        logger.LogError(ex, "Error processing the order message.");
                    }
                    // Output error to console for debugging
                    Console.WriteLine($"Error processing order message: {ex.Message}");

                    // Still acknowledge the message to prevent re-delivery
                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                }
            };

            // Start consuming messages from the queue without auto-acknowledge
            channel.BasicConsumeAsync(queue: queueName,
                                  autoAck: false,
                                  consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            channel?.CloseAsync();
            channel?.Dispose();

            // Log worker stopping and channel closing with correlation ID
            using (LogContext.PushProperty("CorrelationId", Guid.NewGuid().ToString()))
            {
                logger.LogInformation("Stopping Worker and closing the RabbitMQ channel.");
            }
            base.Dispose();
        }
    }
}
