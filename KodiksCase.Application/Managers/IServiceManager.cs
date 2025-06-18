using KodiksCase.Application.Services.Interfaces;
using KodiksCase.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Application.Managers
{
    public interface IServiceManager
    {
        IOrderService OrderService { get; }
        IUserService UserService { get; }
        IProductService ProductService { get; }
        ITokenService TokenService { get; }
        IMessagePublisher MessagePublisher { get; }
    }
}
