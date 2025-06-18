using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Infrastructure.Caching
{
    public interface IRedisCacheService
    {
        Task SetProcessingLogAsync(string key, string value, TimeSpan? expiry = null);
        Task<string?> GetProcessingLogAsync(string key);
        Task RemoveAsync(string key);
    }
}
