using KodiksCase.Application.Services.Interfaces;
using KodiksCase.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Application.Managers
{
    // ServiceManager acts as a centralized facade to expose and manage all application services.
    // It provides a single access point for various service interfaces, simplifying dependency injection and usage.
    public class ServiceManager : IServiceManager
    {
        public ServiceManager(
            IOrderService orderService,
            IUserService userService,
            IProductService productService,
            ITokenService tokenService,
            IMessagePublisher messagePublisher)
        {
            OrderService = orderService;
            UserService = userService;
            ProductService = productService;
            TokenService = tokenService;
            MessagePublisher= messagePublisher;
        }
        public IOrderService OrderService { get; }
        public IUserService UserService { get; }
        public IProductService ProductService { get; }
        public ITokenService TokenService { get; }
        public IMessagePublisher MessagePublisher { get; }
    }
}
