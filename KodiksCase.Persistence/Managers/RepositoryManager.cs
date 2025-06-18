using KodiksCase.Persistence.Context;
using KodiksCase.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Persistence.Managers
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly KodiksCaseDbContext context;
        public RepositoryManager(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IProductRepository productRepository,
            KodiksCaseDbContext context)
        {
            OrderRepository = orderRepository;
            UserRepository = userRepository;
            ProductRepository = productRepository;
            this.context = context;
        }

        public IOrderRepository OrderRepository { get; }
        public IUserRepository UserRepository { get; }
        public IProductRepository ProductRepository { get; }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
