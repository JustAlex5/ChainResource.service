using ChainResource.service.DAL.Interfaces;

namespace ChainResource.service.DAL;

public class StorageFactory<T>:IStorageFactory<T>
{
    private readonly MemoryDal<T> _memoryDal;
    private readonly FileSystemDal<T> _fileSystemDal;
    private readonly WebServiceDal<T> _webServiceDal;
    private readonly ILogger<StorageFactory<T>> _logger;

    public StorageFactory(MemoryDal<T> memoryDal, FileSystemDal<T> fileSystemDal,WebServiceDal<T> webServiceDal, ILogger<StorageFactory<T>> logger)
    {
        _memoryDal = memoryDal;
        _fileSystemDal = fileSystemDal;
        _webServiceDal = webServiceDal;
        _logger = logger;
    }
    public IEnumerable<IStorageDal<T>> GetRetrievalOrder()
    {
        yield return _memoryDal;
        yield return _fileSystemDal;
        yield return _webServiceDal;
    }
    
    public async Task UpdateAllAsync(T value)
    {
        var tasks = new List<Task>();
        foreach (var dal in GetRetrievalOrder())
        {
            if (dal.IsWritable)
            {
                tasks.Add(dal.WrtiAsync(value));
            }
        }
        await Task.WhenAll(tasks);
    }
}