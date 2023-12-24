using System.Text.Json;
using Application.DataAccess;
using StackExchange.Redis;
using Throw;

namespace Infrastructure.Cache;

public class RedisCacheService: ICacheService
{
    private readonly IDatabase _cacheDb;

    private const int SecondsExpirationTime = 15;
    
    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _cacheDb = redis.GetDatabase();
    }
    
    public Task<T?> GetDataAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        key.Throw().IfNullOrWhiteSpace(x => x);
        
        var value = _cacheDb.StringGet(key);
        return !value.IsNullOrEmpty
            ? Task.FromResult(JsonSerializer.Deserialize<T>(value))
            : Task.FromResult<T>(default);
    }

    public Task<bool> SetDataAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        key.Throw().IfNullOrWhiteSpace(x => x);
        
        var isSucceed = _cacheDb.StringSet(
            key,
            JsonSerializer.Serialize(value),
            TimeSpan.FromSeconds(SecondsExpirationTime)
            );
        
        return Task.FromResult(isSucceed);
    }

    public Task<bool> GetOrSetDataAsync<T>(string key, ref T output, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveDataAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}