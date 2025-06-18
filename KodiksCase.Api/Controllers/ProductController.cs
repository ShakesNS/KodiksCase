using KodiksCase.Application.Managers;
using KodiksCase.Shared.DTOs.Requests;
using KodiksCase.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KodiksCase.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IServiceManager serviceManager;

        public ProductController(IServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <returns>
        /// Returns 200 OK with the product details if found.
        /// Returns 404 Not Found if the product does not exist.
        /// Returns 400 Bad Request if an error occurs.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            try
            {
                var product = await serviceManager.ProductService.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound(ApiResponse<string>.Fail("Product not found"));

                return Ok(ApiResponse<object>.SuccessResponse(product));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves the list of all available products.
        /// </summary>
        /// <returns>
        /// Returns 200 OK with the list of products if any exist.
        /// Returns 404 Not Found if no products are available.
        /// Returns 400 Bad Request if an error occurs.
        /// </returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await serviceManager.ProductService.GetProductsAsync();
                if (products == null)
                    return NotFound(ApiResponse<string>.Fail("Products not found"));

                return Ok(ApiResponse<object>.SuccessResponse(products));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Creates a new product with the provided details.
        /// </summary>
        /// <param name="request">The product data to create.</param>
        /// <returns>
        /// Returns 200 OK with the new product ID if creation is successful.
        /// Returns 400 Bad Request if the request data is invalid or an error occurs.
        /// </returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Invalid product data."));
            try
            {
                var productId = await serviceManager.ProductService.CreateProductAsync(request);
                return Ok(ApiResponse<object>.SuccessResponse(new { productId }, "Product created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }
    }
}
