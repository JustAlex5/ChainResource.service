using ChainResource.service.BL.Interfaces;
using ChainResource.service.DAL.Interfaces;
using ChainResource.service.Model;

namespace ChainResource.service.BL;

public class ChainResource<T>:IChainResource<T>
{
    private readonly IStorageFactory<T> _storageFactory;
    private readonly ILogger<ChainResource<T>> _logger;

    public ChainResource(ILogger<ChainResource<T>> logger, IStorageFactory<T> storageFactory)
    {
        _logger = logger;
        _storageFactory = storageFactory;
        
    }
    public async Task<T> GetValue()
    {
        var chain = _storageFactory.GetRetrievalOrder().ToList();

        for(int i = 0; i < chain.Count; i++)
        {
            try
            {
                var (value,isValid) = await chain[i].TryReadAsync();
                if (isValid)
                {
                    _logger.LogInformation("Value retrieved from {DalType}", chain[i].GetType().Name);
                        await UpdateAboveAsync(chain, value, i);
                    return value;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving value from {DalType}", chain[i].GetType().Name);
            }
        }
        throw new Exception("Unable to retrieve value from any storage.");
    }
    
    private async Task UpdateAboveAsync(List<IStorageDal<T>> chain,T value, int index)
    {
        for (int i = 0; i < index; i++)
        {
            if (chain[i].IsWritable)
            {
                await chain[i].WrtiAsync(value);
            }
        }
        
    }
}