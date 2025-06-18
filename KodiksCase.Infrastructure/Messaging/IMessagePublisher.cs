using KodiksCase.Shared.DTOs.RabbitMq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Infrastructure.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishOrderPlacedAsync(RabbitMqOrderDto order);
    }
}
