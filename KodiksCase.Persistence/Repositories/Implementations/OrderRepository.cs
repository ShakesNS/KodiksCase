using KodiksCase.Persistence.Context;
using KodiksCase.Persistence.Repositories.Interfaces;
using KodiksCase.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Persistence.Repositories.Implementations
{
    // Handles data operations specific to Order entities, extending generic repository functionality.
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly KodiksCaseDbContext context;

        public OrderRepository(KodiksCaseDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            return await context.Orders.Where(o => o.UserId == userId).ToListAsync();
        }
    }
}
