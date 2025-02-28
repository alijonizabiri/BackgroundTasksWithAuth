namespace Infrastructure.Interfaces;

public interface IRedisCacheService
{
    Task SetData<T>(string key, T value, int expirationMinutes);
    Task<T?> GetData<T>(string key);
    Task RemoveData(string key);
}
