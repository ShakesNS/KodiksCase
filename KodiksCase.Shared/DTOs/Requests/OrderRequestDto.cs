using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Shared.DTOs.Requests
{
    public class OrderRequestDto
    {
        [Required]
        public string UserId { get; set; } = null!;
        [Required]
        public string ProductId { get; set; } = null!;
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
        [Required]
        [RegularExpression("CreditCard|BankTransfer", ErrorMessage = "PaymentMethod must be 'CreditCard' or 'BankTransfer'.")]
        public string PaymentMethod { get; set; } = null!;
    }
}
