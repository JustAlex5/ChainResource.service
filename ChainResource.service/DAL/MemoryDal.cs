using ChainResource.service.Configuration;
using ChainResource.service.DAL.Interfaces;
using ChainResource.service.Utils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ChainResource.service.DAL;

public class MemoryDal<T> :IStorageDal<T>
{
    private readonly MemoryConfiguration _memoryConfiguration;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryDal<T>> _logger;
    private readonly ILocalCache<string,T> _cache;
    public MemoryDal(IOptions<MemoryConfiguration> memoryConfiguration, 
        IMemoryCache memoryCache, 
        ILogger<MemoryDal<T>> logger,
        ILocalCache<string,T> cache)
    {
        _memoryConfiguration = memoryConfiguration.Value;
        _memoryCache = memoryCache;
        _logger = logger;
        _cache = cache;
    }
    
    public bool IsWritable { get; } = true;

    public  Task<(T value, bool IsValid)> TryReadAsync()
    {
        var key = typeof(T).FullName;
         _ = _cache.TryGetValue<T>(key, out var value);
        if (value != null)
        {
            _logger.LogInformation("[{Method}]Cache hit for type {TypeName}",nameof(TryReadAsync), typeof(T).FullName);
            return Task.FromResult((value, true));
        }
        return Task.FromResult((default(T)!, false));
    }

    public Task WrtiAsync(T value)
    {
        var key = typeof(T).FullName;
        _cache.Set<T>(key , value);
        return Task.CompletedTask;
    }
    
  
}