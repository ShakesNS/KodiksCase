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
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IRabbitMqConnectionManager>(sp =>
            {
                return RabbitMqConnectionManager.CreateAsync(configuration).GetAwaiter().GetResult();
            });

            services.AddSingleton<IMessagePublisher>(sp =>
            {
                var connectionManager = sp.GetRequiredService<IRabbitMqConnectionManager>();
                var queueName = configuration["RabbitMQ:QueueName"];
                return new RabbitMqMessagePublisher(connectionManager, queueName);
            });

            services.AddSingleton<IRedisCacheService>(sp =>
            {
                var redisConnection = configuration.GetConnectionString("Redis");
                return new RedisCacheService(redisConnection);
            });

            return services;
        }
    }
}
