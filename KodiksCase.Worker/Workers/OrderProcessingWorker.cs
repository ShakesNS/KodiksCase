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
            using (LogContext.PushProperty("CorrelationId", Guid.NewGuid().ToString()))
            {
                logger.LogInformation("RabbitMQ channel and queue prepared. Worker started.");
            }
            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var order = JsonSerializer.Deserialize<RabbitMqOrderDto>(message);
                    using (LogContext.PushProperty("CorrelationId", Guid.NewGuid().ToString()))
                    {
                        logger.LogInformation("A new order message has been received. OrderId: {OrderId}", order.OrderId);
                    }

                    await Task.Delay(1000);

                    await redisCacheService.SetProcessingLogAsync($"processed:{order.OrderId}", $"Processed at {DateTime.Now}");
                    using (LogContext.PushProperty("CorrelationId", Guid.NewGuid().ToString()))
                    {
                        logger.LogInformation("Order processing log was written to Redis.Order processing log was written to Redis.A new order message was received. OrderId: {OrderId}", order.OrderId);
                    }

                    var log = await redisCacheService.GetProcessingLogAsync($"processed:{order.OrderId}");
                    using (LogContext.PushProperty("CorrelationId", Guid.NewGuid().ToString()))
                    {
                        logger.LogInformation("Processing log read from Redis: {Log}", log);
                    }
                    using (LogContext.PushProperty("CorrelationId", Guid.NewGuid().ToString()))
                    {
                        logger.LogInformation("Order processed successfully. OrderId: {OrderId}", order.OrderId);
                    }
                    Console.WriteLine($"Redis log: {log}");

                    Console.WriteLine($"Order processed: {order.OrderId} at {DateTime.Now}");


                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    using (LogContext.PushProperty("CorrelationId", Guid.NewGuid().ToString()))
                    {
                        logger.LogError(ex, "Error processing the order message.");
                    }
                    Console.WriteLine($"Error processing order message: {ex.Message}");

                    // Hata durumunda mesajı kuyrukta bırak (requeue)
                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                }
            };

            channel.BasicConsumeAsync(queue: queueName,
                                  autoAck: false,
                                  consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            channel?.CloseAsync();
            channel?.Dispose();
            using (LogContext.PushProperty("CorrelationId", Guid.NewGuid().ToString()))
            {
                logger.LogInformation("Stopping Worker and closing the RabbitMQ channel.");
            }
            base.Dispose();
        }
    }
}
