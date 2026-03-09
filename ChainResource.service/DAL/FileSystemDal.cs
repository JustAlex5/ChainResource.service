using System.Text.Json;
using ChainResource.service.Configuration;
using ChainResource.service.DAL.Interfaces;
using Microsoft.Extensions.Options;

namespace ChainResource.service.DAL;

public class FileSystemDal<T> : IStorageDal<T>
{
    private readonly FileSystemConfiguration _fileSystemConfiguration;
    private readonly ILogger<FileSystemDal<T>> _logger;
    private TimeSpan _expiration;

    public FileSystemDal(IOptions<FileSystemConfiguration> fileSystemConfiguration, ILogger<FileSystemDal<T>> logger)
    {
        _fileSystemConfiguration = fileSystemConfiguration.Value;
        _logger = logger;
        _expiration = _fileSystemConfiguration.TTLTimeSpan;    
    }
    public bool IsWritable { get; } = true;

    public async Task<(T value, bool IsValid)> TryReadAsync()
    {
        if (!File.Exists(_fileSystemConfiguration.FilePath))
            return  (default, false);
        var lastWrite = File.GetLastWriteTimeUtc(_fileSystemConfiguration.FilePath);
        if (DateTime.UtcNow - lastWrite >= _expiration)
            return (default,false);
        var json = await File.ReadAllTextAsync(_fileSystemConfiguration.FilePath);
        var res = JsonSerializer.Deserialize<T>(json);
        return (res, true);



    }

    public async Task WrtiAsync(T value)
    {
        var dir = Path.GetDirectoryName(_fileSystemConfiguration.FilePath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        var json = JsonSerializer.Serialize<T>(value);
        await File.WriteAllTextAsync(_fileSystemConfiguration.FilePath, json);
    }
}