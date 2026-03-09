namespace ChainResource.service.DAL.Interfaces;

public interface IStorageFactory<T>
{
    IEnumerable<IStorageDal<T>> GetRetrievalOrder();
    Task UpdateAllAsync(T value);

}