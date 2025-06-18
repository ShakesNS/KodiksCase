using KodiksCase.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Persistence.Managers
{
    public interface IRepositoryManager
    {
        IOrderRepository OrderRepository { get; }
        IUserRepository UserRepository { get; }
        IProductRepository ProductRepository { get; }
        Task SaveAsync();
    }
}
