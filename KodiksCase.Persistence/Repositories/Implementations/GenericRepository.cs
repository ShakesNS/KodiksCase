using KodiksCase.Persistence.Context;
using KodiksCase.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Persistence.Repositories.Implementations
{
    // Provides a generic repository implementation to handle common CRUD operations for any entity type.
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly KodiksCaseDbContext context;
        private readonly DbSet<T> dbSet;

        public GenericRepository(KodiksCaseDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(Guid id) => await dbSet.FindAsync(id);

        public async Task AddAsync(T entity) => await dbSet.AddAsync(entity);

        public void Update(T entity) => dbSet.Update(entity);

        public void Delete(T entity) => dbSet.Remove(entity);
    }
}
