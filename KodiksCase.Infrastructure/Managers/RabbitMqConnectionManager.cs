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
    // Manages RabbitMQ connection and channel creation, handling resource disposal.
    public class RabbitMqConnectionManager : IRabbitMqConnectionManager
    {
        private readonly IConnection connection;
        public RabbitMqConnectionManager(IConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Creates a new RabbitMQ connection asynchronously using configuration settings.
        /// </summary>
        /// <param name="configuration">Application configuration containing RabbitMQ settings.</param>
        /// <returns>A new instance of RabbitMqConnectionManager with an active connection.</returns>
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

        /// <summary>
        /// Gets the underlying RabbitMQ connection.
        /// </summary>
        /// <returns>The RabbitMQ connection instance.</returns>
        public IConnection GetConnection() => connection;

        /// <summary>
        /// Creates a new channel asynchronously on the RabbitMQ connection.
        /// </summary>
        /// <returns>A task representing the asynchronous creation of a channel.</returns>
        public async Task<IChannel> CreateChannel() => await connection.CreateChannelAsync();

        /// <summary>
        /// Synchronously disposes the RabbitMQ connection.
        /// </summary>
        public void Dispose()
        {
            connection.CloseAsync();
            connection?.Dispose();
        }

        /// <summary>
        /// Asynchronously disposes the RabbitMQ connection.
        /// </summary>
        /// <returns>A ValueTask representing the asynchronous dispose operation.</returns>
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
