using KodiksCase.Shared.DTOs.Requests;
using KodiksCase.Shared.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Guid> CreateOrderAsync(OrderRequestDto request);
        Task<IEnumerable<OrderResponseDto>> GetOrdersByUserIdAsync(Guid userId);
    }
}
