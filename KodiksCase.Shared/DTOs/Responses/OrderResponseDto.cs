using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Shared.DTOs.Responses
{
    public class OrderResponseDto
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
