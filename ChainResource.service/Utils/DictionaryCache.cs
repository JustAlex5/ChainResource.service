using System.Collections.Concurrent;
using ChainResource.service.Configuration;
using ChainResource.service.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ChainResource.service.Utils;

public class DictionaryCache<TKey,TValue>: ILocalCache <TKey,TValue> where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, TtlValue<TValue>> _cache = new();
    private readonly TimeSpan _expiration;
    public DictionaryCache(IOptions<MemoryConfiguration> options)
    {
        _expiration = options.Value.TTLTimeSpan;
    }
    public void Set<T>(TKey key, TValue value)
    {
        var ttlValue = new TtlValue<TValue>
        {
            Data = value,
            Expiration = DateTime.UtcNow.Add(_expiration)
        };
        _cache[key] = ttlValue;
    }
    
    public bool TryGetValue<T>(TKey key, out TValue value)
    {
        if (_cache.TryGetValue(key, out var cachedValue) && !cachedValue.IsExpired)
        {
            {
                value = cachedValue.Data;   
                return true;
            }
        }

        if (cachedValue != null && cachedValue.IsExpired)
        {
            Remove<T>(key);
        }
        value = default!;
        return false;
    }

    private void Remove<T>(TKey key)
    {
        _cache.TryRemove(key, out _);
    }
    // This function CleaAll() Is can be useful api such ass api/ClearCache
    public void ClearAll() =>      _cache.Clear();


}