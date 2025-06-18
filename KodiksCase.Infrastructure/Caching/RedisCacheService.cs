using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Infrastructure.Caching
{
    /// <summary>
    /// Service for interacting with Redis cache, providing methods to set, get, and remove cached data.
    /// Implements IDisposable to properly dispose Redis connection.
    /// </summary>
    public class RedisCacheService : IRedisCacheService, IDisposable
    {
        private readonly ConnectionMultiplexer redis;
        private readonly StackExchange.Redis.IDatabase db;

        /// <summary>
        /// Initializes Redis connection and database instance with the provided connection string.
        /// </summary>
        /// <param name="connectionString">Redis connection string.</param>
        public RedisCacheService(string connectionString)
        {
            this.redis = ConnectionMultiplexer.Connect(connectionString);
            this.db = redis.GetDatabase();
        }


        /// <summary>
        /// Sets a string value in Redis cache with optional expiration time.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <param name="value">Value to cache.</param>
        /// <param name="expiry">Optional expiration timespan.</param>
        public async Task SetProcessingLogAsync(string key, string value, TimeSpan? expiry = null)
        {
            await db.StringSetAsync(key, value, expiry);
        }

        /// <summary>
        /// Retrieves a cached string value by key from Redis.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <returns>Cached string value or null if not found.</returns>
        public async Task<string?> GetProcessingLogAsync(string key)
        {
            return await db.StringGetAsync(key);
        }

        /// <summary>
        /// Deletes a cached key from Redis.
        /// </summary>
        /// <param name="key">Cache key to remove.</param>
        public async Task RemoveAsync(string key)
        {
            await db.KeyDeleteAsync(key);
        }

        /// <summary>
        /// Disposes the Redis connection when the service is disposed.
        /// </summary>
        public void Dispose()
        {
            redis?.Dispose();
        }

    }
}
