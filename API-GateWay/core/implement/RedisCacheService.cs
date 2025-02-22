using API_GateWay.core.Services;
using Microsoft.Extensions.Caching.Distributed;
namespace API_GateWay.core.implement;

public class RedisCacheService(IDistributedCache cache) : IRedisCacheService
{
    public async Task<string?> GetCachedResponseAsync(string key)
    {
        return await cache.GetStringAsync(key);
    }

    public async Task SetCachedResponseAsync(string key, string response, TimeSpan duration)
    {
        await cache.SetStringAsync(key, response, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = duration
        });
    }
}