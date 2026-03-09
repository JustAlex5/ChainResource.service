namespace ChainResource.service.Configuration;

public class FileSystemConfiguration
{
    public const string ConfigName = "FileSystemConfiguration";
    public int TTL { get; set; } = 14400;
    public string FilePath  { get; set; }
    public TimeSpan TTLTimeSpan => TimeSpan.FromSeconds(TTL);
}