using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Infrastructure.Caching
{
    public class RedisCacheService : IRedisCacheService, IDisposable
    {
        private readonly ConnectionMultiplexer redis;
        private readonly StackExchange.Redis.IDatabase db;
        public RedisCacheService(string connectionString)
        {
            this.redis = ConnectionMultiplexer.Connect(connectionString);
            this.db = redis.GetDatabase();
        }

        public async Task SetProcessingLogAsync(string key, string value, TimeSpan? expiry = null)
        {
            await db.StringSetAsync(key, value, expiry);
        }
        public async Task<string?> GetProcessingLogAsync(string key)
        {
            return await db.StringGetAsync(key);
        }
        public void Dispose()
        {
            redis?.Dispose();
        }
        public async Task RemoveAsync(string key)
        {
            await db.KeyDeleteAsync(key);
        }
    }
}
