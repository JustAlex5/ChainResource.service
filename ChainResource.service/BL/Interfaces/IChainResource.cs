namespace ChainResource.service.BL.Interfaces;

public interface IChainResource<T>
{
    Task<T> GetValue();
}