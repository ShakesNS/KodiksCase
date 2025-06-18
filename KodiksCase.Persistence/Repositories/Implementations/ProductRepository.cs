using KodiksCase.Persistence.Context;
using KodiksCase.Persistence.Repositories.Interfaces;
using KodiksCase.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Persistence.Repositories.Implementations
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly KodiksCaseDbContext context;

        public ProductRepository(KodiksCaseDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}
