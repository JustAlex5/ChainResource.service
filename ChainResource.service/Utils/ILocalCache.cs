namespace ChainResource.service.Utils;

public interface ILocalCache<TKey, TValue> where TKey : notnull
{
    public void Set<T>(TKey key, TValue value);
    public bool TryGetValue<T>(TKey key, out TValue value);
    public void ClearAll();

}