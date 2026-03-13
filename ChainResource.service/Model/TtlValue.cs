namespace ChainResource.service.Model;

public class TtlValue<T>
{
    public T Data { get; set; }
    public DateTime Expiration { get; set; }
    public bool IsExpired => DateTime.UtcNow > Expiration;
}