using ChainResource.service.Configuration;
using ChainResource.service.DAL.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ChainResource.service.DAL;

public class MemoryDal<T> :IStorageDal<T>
{
    private readonly MemoryConfiguration _memoryConfiguration;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryDal<T>> _logger;
    public MemoryDal(IOptions<MemoryConfiguration> memoryConfiguration, IMemoryCache memoryCache, ILogger<MemoryDal<T>> logger)
    {
        _memoryConfiguration = memoryConfiguration.Value;
        _memoryCache = memoryCache;
        _logger = logger;
    }
    
    public bool IsWritable { get; } = true;

    public async Task<(T value, bool IsValid)> TryReadAsync()
    {
        var value = await GetCache(typeof(T).FullName);
        if (value != null)
        {
            return (value, true);
        }
        return (default, false);
    }

    public async Task WrtiAsync(T value)
    {
        await SetCache(value);
    }
    
    private Task SetCache(T value)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(_memoryConfiguration.TTL));
         _memoryCache.Set(typeof(T).FullName, value, cacheEntryOptions);
            return Task.CompletedTask;
    }

    private Task<T?> GetCache(string key)
    {
        if ( _memoryCache.TryGetValue(key, out T? value))
        {
            return Task.FromResult<T>(value);
        }
        return Task.FromResult<T?>(default);
        
    }
}