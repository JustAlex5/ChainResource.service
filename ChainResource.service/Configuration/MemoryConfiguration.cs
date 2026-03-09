namespace ChainResource.service.Configuration;

public class MemoryConfiguration
{
    public const string ConfigName = "MemoryConfiguration";
    public int TTL { get; set; } = 3600;
    public TimeSpan TTLTimeSpan => TimeSpan.FromSeconds(TTL);
}