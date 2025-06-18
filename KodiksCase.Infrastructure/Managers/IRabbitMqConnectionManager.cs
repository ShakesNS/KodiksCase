using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Infrastructure.Managers
{
    public interface IRabbitMqConnectionManager : IAsyncDisposable, IDisposable
    {
        IConnection GetConnection();
        Task<IChannel> CreateChannel();
    }
}
