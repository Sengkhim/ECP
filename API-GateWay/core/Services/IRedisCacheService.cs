namespace API_GateWay.core.Services;

public interface IRedisCacheService
{
    Task<string?> GetCachedResponseAsync(string key);
    Task SetCachedResponseAsync(string key, string response, TimeSpan duration);
}
