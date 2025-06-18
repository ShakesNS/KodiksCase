using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Infrastructure.Managers
{
    public class RabbitMqConnectionManager : IRabbitMqConnectionManager
    {
        private readonly IConnection connection;
        public RabbitMqConnectionManager(IConnection connection)
        {
            this.connection = connection;
        }
        public static async Task<RabbitMqConnectionManager> CreateAsync(IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };

            var connection = await factory.CreateConnectionAsync();
            return new RabbitMqConnectionManager(connection);
        }

        public IConnection GetConnection() => connection;

        public async Task<IChannel> CreateChannel() => await connection.CreateChannelAsync();

        public void Dispose()
        {
            connection.CloseAsync();
            connection?.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (connection != null)
            {
                await connection.CloseAsync();
                connection.Dispose();
                await Task.CompletedTask;
            }
        }
    }
}
