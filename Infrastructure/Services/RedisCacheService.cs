using System.Text.Json;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Services;

public class RedisCacheService(IDistributedCache cache) : IRedisCacheService
{
    public async Task SetData<T>(string key, T value, int expirationMinutes)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationMinutes),
        };
        var jsonSerializerOption = new JsonSerializerOptions { WriteIndented = true };
        
        var jsonData = JsonSerializer.Serialize(value, jsonSerializerOption);
        await cache.SetStringAsync(key, jsonData, options);
    }
    
    public async Task<T?> GetData<T>(string key)
    {
        var jsonData = await cache.GetStringAsync(key);
        
        return jsonData != null 
            ? JsonSerializer.Deserialize<T>(jsonData) 
            : default;
    }

    public async Task RemoveData(string key)
    {
        await cache.RemoveAsync(key);
    }
}
