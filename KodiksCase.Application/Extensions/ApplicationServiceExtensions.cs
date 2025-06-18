using KodiksCase.Application.Managers;
using KodiksCase.Application.Services.Implementations;
using KodiksCase.Application.Services.Interfaces;
using KodiksCase.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Application.Extensions
{
    // Extension method to register application services and configurations into the DI container
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}