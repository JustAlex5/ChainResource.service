namespace ChainResource.service.DAL.Interfaces;

public interface IStorageDal<T>
{
    bool IsWritable { get; }
    Task<(T value, bool IsValid)> TryReadAsync();
    Task WrtiAsync(T value);
}