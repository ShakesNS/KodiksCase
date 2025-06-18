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
    /// <summary>
    /// Handles all business logic and operations related to orders.
    /// Provides methods to create orders, retrieve user orders, and manage order-related processes.
    /// </summary>
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

        /// <summary>
        /// Creates a new order based on the provided order request data.
        /// Validates the payment method, user ID, and product ID.
        /// Persists the order, clears related cache, and publishes an order placed message.
        /// </summary>
        /// <param name="request">The order request DTO containing order details.</param>
        /// <returns>The unique identifier of the created order.</returns>
        /// <exception cref="ArgumentException">Thrown when payment method, user ID, or product ID is invalid.</exception>
        /// <exception cref="Exception">Thrown when user or product is not found.</exception>
        public async Task<Guid> CreateOrderAsync(OrderRequestDto request)
        {
            // Validate and parse PaymentMethod enum from request
            if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, true, out var paymentMethod))
                throw new ArgumentException("Invalid payment method");

            // Validate and parse UserId GUID from request
            if (!Guid.TryParse(request.UserId, out var userId))
                throw new ArgumentException("Invalid UserId format.");

            // Validate and parse ProductId GUID from request
            if (!Guid.TryParse(request.ProductId, out var productId))
                throw new ArgumentException("Invalid ProductId format.");

            // Retrieve user entity by UserId
            var user = await repositoryManager.UserRepository.GetByIdAsync(Guid.Parse(request.UserId));
            if (user == null) 
                throw new Exception("User not found");

            // Retrieve product entity by ProductId
            var product = await repositoryManager.ProductRepository.GetByIdAsync(Guid.Parse(request.ProductId));
            if (product == null) 
                throw new Exception("Product not found");

            // Create new Order entity with provided details
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ProductId = product.Id,
                Quantity = request.Quantity,
                PaymentMethod = paymentMethod,
                CreatedAt = DateTime.Now
            };

            // Add order to repository and persist changes
            await repositoryManager.OrderRepository.AddAsync(order);
            await repositoryManager.SaveAsync();

            // Log order creation with key details
            logger.LogInformation("Order created successfully. OrderId: {OrderId}, UserId: {UserId}, ProductId: {ProductId}, Quantity: {Quantity}, PaymentMethod: {PaymentMethod}",
                order.Id, order.UserId, order.ProductId, order.Quantity, order.PaymentMethod.ToString());

            // Clear cached orders for the user
            await redisCacheService.RemoveAsync($"orders:{order.UserId}");

            // Prepare order DTO for RabbitMQ message
            var orderDto = new RabbitMqOrderDto
            {
                OrderId = order.Id,
                UserId = order.UserId,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                PaymentMethod = order.PaymentMethod.ToString(),
                CreatedAt = order.CreatedAt
            };

            // Publish order placed event to RabbitMQ
            await messagePublisher.PublishOrderPlacedAsync(orderDto);

            // Return the created order ID
            return order.Id;
        }

        /// <summary>
        /// Retrieves all orders associated with a specific user.
        /// Attempts to return cached data first; if not present, queries the database,
        /// caches the result, and returns the list of order response DTOs.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose orders are requested.</param>
        /// <returns>A collection of order response DTOs belonging to the user.</returns>
        public async Task<IEnumerable<OrderResponseDto>> GetOrdersByUserIdAsync(Guid userId)
        {
            // Define cache key for user orders
            string cacheKey = $"orders:{userId}";

            // Try to get cached order data from Redis
            var cachedData = await redisCacheService.GetProcessingLogAsync(cacheKey);

            // If cached data exists, deserialize and return it
            if (!string.IsNullOrEmpty(cachedData))
            {
                var cachedOrders = JsonSerializer.Deserialize<IEnumerable<OrderResponseDto>>(cachedData);
                if (cachedOrders != null)
                    return cachedOrders;
            }

            // Fetch orders from repository if no cache found
            var orders = await repositoryManager.OrderRepository
                .GetOrdersByUserIdAsync(userId);

            // Return empty list if no orders found
            if (orders == null || !orders.Any())
                return Enumerable.Empty<OrderResponseDto>();

            // Map orders to DTOs
            var orderDtos = orders.Select(o => new OrderResponseDto
            {
                OrderId = o.Id,
                ProductId = o.ProductId,
                Quantity = o.Quantity,
                PaymentMethod = o.PaymentMethod.ToString(),
                CreatedAt = o.CreatedAt
            });

            // Serialize DTOs and store them in Redis cache
            var serializedData = JsonSerializer.Serialize(orderDtos);
            await redisCacheService.SetProcessingLogAsync(cacheKey, serializedData, TimeSpan.FromMinutes(2));

            // Return the order DTO list
            return orderDtos;
        }
    }
}
