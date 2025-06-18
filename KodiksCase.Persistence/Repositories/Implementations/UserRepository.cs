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
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly KodiksCaseDbContext context;

        public UserRepository(KodiksCaseDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await context.Users
                .Where(u => u.Email.ToLower() == email.ToLower())
                .FirstOrDefaultAsync();
        }
    }
}
