using KodiksCase.Infrastructure.Caching;
using KodiksCase.Infrastructure.Managers;
using KodiksCase.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Infrastructure.Extensions
{
    // Registers essential infrastructure services like RabbitMQ and Redis for messaging and caching.
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register RabbitMQ connection manager as singleton, initialized asynchronously
            services.AddSingleton<IRabbitMqConnectionManager>(sp =>
            {
                return RabbitMqConnectionManager.CreateAsync(configuration).GetAwaiter().GetResult();
            });

            // Register RabbitMQ message publisher singleton, dependent on connection manager and queue name from config
            services.AddSingleton<IMessagePublisher>(sp =>
            {
                var connectionManager = sp.GetRequiredService<IRabbitMqConnectionManager>();
                var queueName = configuration["RabbitMQ:QueueName"];
                return new RabbitMqMessagePublisher(connectionManager, queueName);
            });

            // Register Redis cache service singleton using Redis connection string from configuration
            services.AddSingleton<IRedisCacheService>(sp =>
            {
                var redisConnection = configuration.GetConnectionString("Redis");
                return new RedisCacheService(redisConnection);
            });

            return services;
        }
    }
}
