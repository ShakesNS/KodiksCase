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
    public class ProductService : IProductService
    {
        private readonly IRepositoryManager repositoryManager;
        private readonly ILogger<ProductService> logger;


        public ProductService(IRepositoryManager repositoryManager, ILogger<ProductService> logger)
        {
            this.repositoryManager = repositoryManager;
            this.logger = logger;
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(Guid productId)
        {
            var product = await repositoryManager.ProductRepository.GetByIdAsync(productId);
            if (product == null)
                return null;

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
            };
        }
        public async Task<IEnumerable<ProductResponseDto?>> GetProductsAsync()
        {
            var products = await repositoryManager.ProductRepository.GetAllAsync();
            return products.Select(p=> new ProductResponseDto{ Id = p.Id, Name = p.Name, Price = p.Price });
        }
        public async Task<Guid> CreateProductAsync(ProductRequestDto productDto)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = productDto.Name,
                Price = productDto.Price
            };

            await repositoryManager.ProductRepository.AddAsync(product);
            await repositoryManager.SaveAsync();
            logger.LogInformation("Product created successfully: Id={ProductId}, Name={Name}, Price={Price}", product.Id, product.Name, product.Price);
            return product.Id;
        }
    }
}
