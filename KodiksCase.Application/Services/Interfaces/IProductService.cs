using KodiksCase.Shared.DTOs.Requests;
using KodiksCase.Shared.DTOs.Responses;
using KodiksCase.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDto?> GetProductByIdAsync(Guid productId);
        Task<Guid> CreateProductAsync(ProductRequestDto productDto);
        Task<IEnumerable<ProductResponseDto?>> GetProductsAsync();
    }
}
