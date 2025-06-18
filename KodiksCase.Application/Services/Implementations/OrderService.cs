using KodiksCase.Application.Services.Interfaces;
using KodiksCase.Infrastructure.Caching;
using KodiksCase.Infrastructure.Messaging;
using KodiksCase.Persistence.Managers;
using KodiksCase.Shared.DTOs.RabbitMq;
using KodiksCase.Shared.DTOs.Requests;
using KodiksCase.Shared.DTOs.Responses;
using KodiksCase.Shared.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KodiksCase.Application.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IRepositoryManager repositoryManager;
        private readonly IMessagePublisher messagePublisher;
        private readonly IRedisCacheService redisCacheService;
        private readonly ILogger<OrderService> logger;
        public OrderService(IRepositoryManager repositoryManager, IMessagePublisher messagePublisher, IRedisCacheService redisCacheService, ILogger<OrderService> logger)
        {
            this.repositoryManager = repositoryManager;
            this.messagePublisher = messagePublisher;
            this.redisCacheService = redisCacheService;
            this.logger = logger;
        }

        public async Task<Guid> CreateOrderAsync(OrderRequestDto request)
        {
            if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, true, out var paymentMethod))
                throw new ArgumentException("Invalid payment method");

            if (!Guid.TryParse(request.UserId, out var userId))
                throw new ArgumentException("Invalid UserId format.");

            if (!Guid.TryParse(request.ProductId, out var productId))
                throw new ArgumentException("Invalid ProductId format.");

            var user = await repositoryManager.UserRepository.GetByIdAsync(Guid.Parse(request.UserId));
            if (user == null) 
                throw new Exception("User not found");

            var product = await repositoryManager.ProductRepository.GetByIdAsync(Guid.Parse(request.ProductId));
            if (product == null) 
                throw new Exception("Product not found");

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ProductId = product.Id,
                Quantity = request.Quantity,
                PaymentMethod = paymentMethod,
                CreatedAt = DateTime.Now
            };

            await repositoryManager.OrderRepository.AddAsync(order);
            await repositoryManager.SaveAsync();
            logger.LogInformation("Order created successfully. OrderId: {OrderId}, UserId: {UserId}, ProductId: {ProductId}, Quantity: {Quantity}, PaymentMethod: {PaymentMethod}",
                order.Id, order.UserId, order.ProductId, order.Quantity, order.PaymentMethod.ToString());
            await redisCacheService.RemoveAsync($"orders:{order.UserId}");

            var orderDto = new RabbitMqOrderDto
            {
                OrderId = order.Id,
                UserId = order.UserId,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                PaymentMethod = order.PaymentMethod.ToString(),
                CreatedAt = order.CreatedAt
            };

            await messagePublisher.PublishOrderPlacedAsync(orderDto);

            return order.Id;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersByUserIdAsync(Guid userId)
        {
            string cacheKey = $"orders:{userId}";

            var cachedData = await redisCacheService.GetProcessingLogAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                var cachedOrders = JsonSerializer.Deserialize<IEnumerable<OrderResponseDto>>(cachedData);
                if (cachedOrders != null)
                    return cachedOrders;
            }

            var orders = await repositoryManager.OrderRepository
                .GetOrdersByUserIdAsync(userId);

            if (orders == null || !orders.Any())
                return Enumerable.Empty<OrderResponseDto>();

            var orderDtos = orders.Select(o => new OrderResponseDto
            {
                OrderId = o.Id,
                ProductId = o.ProductId,
                Quantity = o.Quantity,
                PaymentMethod = o.PaymentMethod.ToString(),
                CreatedAt = o.CreatedAt
            });

            var serializedData = JsonSerializer.Serialize(orderDtos);
            await redisCacheService.SetProcessingLogAsync(cacheKey, serializedData, TimeSpan.FromMinutes(2));

            return orderDtos;
        }
    }
}
