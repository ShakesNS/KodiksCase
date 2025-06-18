using KodiksCase.Application.Managers;
using KodiksCase.Persistence.Context;
using KodiksCase.Shared.DTOs.Requests;
using KodiksCase.Shared.DTOs.Responses;
using KodiksCase.Shared.Models;
using KodiksCase.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KodiksCase.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        private readonly IServiceManager serviceManager;
        public OrdersController(IServiceManager serviceManager, ILogger<OrdersController> logger)
        {
            this.serviceManager = serviceManager;
        }

        /// <summary>
        /// Handles the creation of a new order.
        /// Validates the incoming order data and delegates order creation to the service layer.
        /// </summary>
        /// <param name="request">The order details sent in the request body.</param>
        /// <returns>
        /// Returns 200 OK with the newly created order ID on success.
        /// Returns 400 Bad Request with an error message if validation fails or an exception occurs.
        /// </returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Invalid order data."));
            try
            {
                var orderId = await serviceManager.OrderService.CreateOrderAsync(request);
                return Ok(ApiResponse<object>.SuccessResponse(new { orderId }, "Order created successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves all orders associated with the specified user ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>
        /// Returns 200 OK with the list of orders if found.
        /// Returns 400 Bad Request if the userId format is invalid or an error occurs.
        /// Returns 404 Not Found if no orders are found for the given user.
        /// </returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(string userId)
        {
            if (!Guid.TryParse(userId, out var guidUserId))
                return BadRequest(ApiResponse<string>.Fail("Invalid userId format"));
            try
            {
                var orders = await serviceManager.OrderService.GetOrdersByUserIdAsync(guidUserId);

                if (orders == null || !orders.Any())
                    return NotFound(ApiResponse<string>.Fail("Orders not found"));

                return Ok(ApiResponse<IEnumerable<OrderResponseDto>>.SuccessResponse(orders));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }
    }
}
