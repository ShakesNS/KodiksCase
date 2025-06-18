using KodiksCase.Application.Services.Interfaces;
using KodiksCase.Persistence.Managers;
using KodiksCase.Shared.DTOs.Requests;
using KodiksCase.Shared.DTOs.Responses;
using KodiksCase.Shared.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Application.Services.Implementations
{
    /// <summary>
    /// Provides business logic related to product management,
    /// including creating products and retrieving product information.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IRepositoryManager repositoryManager;
        private readonly ILogger<ProductService> logger;
        public ProductService(IRepositoryManager repositoryManager, ILogger<ProductService> logger)
        {
            this.repositoryManager = repositoryManager;
            this.logger = logger;
        }

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="productId">The unique ID of the product.</param>
        /// <returns>The product details as a DTO, or null if not found.</returns>
        public async Task<ProductResponseDto?> GetProductByIdAsync(Guid productId)
        {
            // Retrieve product by ID from repository
            var product = await repositoryManager.ProductRepository.GetByIdAsync(productId);
            if (product == null)
                return null;

            // Map product entity to response DTO
            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
            };
        }

        /// <summary>
        /// Retrieves all products available in the repository.
        /// </summary>
        /// <returns>A collection of product response DTOs.</returns>
        public async Task<IEnumerable<ProductResponseDto?>> GetProductsAsync()
        {
            // Retrieve all products from repository
            var products = await repositoryManager.ProductRepository.GetAllAsync();

            // Map product entities to response DTOs
            return products.Select(p=> new ProductResponseDto
            { 
                Id = p.Id,
                Name = p.Name,
                Price = p.Price 
            });
        }

        /// <summary>
        /// Creates a new product based on the provided product request DTO.
        /// </summary>
        /// <param name="productDto">The DTO containing new product data.</param>
        /// <returns>The unique identifier of the newly created product.</returns>
        public async Task<Guid> CreateProductAsync(ProductRequestDto productDto)
        {
            // Create new product entity from request DTO
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = productDto.Name,
                Price = productDto.Price
            };

            // Add product to repository and save changes
            await repositoryManager.ProductRepository.AddAsync(product);
            await repositoryManager.SaveAsync();

            // Log successful product creation with details
            logger.LogInformation("Product created successfully: Id={ProductId}, Name={Name}, Price={Price}", product.Id, product.Name, product.Price);

            // Return the newly created product's ID
            return product.Id;
        }
    }
}
