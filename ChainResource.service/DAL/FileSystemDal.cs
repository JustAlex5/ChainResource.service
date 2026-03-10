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
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public FileSystemDal(IOptions<FileSystemConfiguration> fileSystemConfiguration, ILogger<FileSystemDal<T>> logger)
    {
        _fileSystemConfiguration = fileSystemConfiguration.Value;
        _logger = logger;
        _expiration = _fileSystemConfiguration.TTLTimeSpan;
    }

    public bool IsWritable { get; } = true;

    public async Task<(T value, bool IsValid)> TryReadAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (!File.Exists(_fileSystemConfiguration.FilePath))
                return (default, false);
            var lastWrite = File.GetLastWriteTimeUtc(_fileSystemConfiguration.FilePath);
            if (DateTime.UtcNow - lastWrite >= _expiration)
                return (default, false);
            var json = await File.ReadAllTextAsync(_fileSystemConfiguration.FilePath);
            var res = JsonSerializer.Deserialize<T>(json);
            return (res, true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occurred while trying to read from file system.");
            return (default, false);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task WrtiAsync(T value)
    {
        await _semaphore.WaitAsync();
        try
        {
            var dir = Path.GetDirectoryName(_fileSystemConfiguration.FilePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var json = JsonSerializer.Serialize<T>(value);
            await File.WriteAllTextAsync(_fileSystemConfiguration.FilePath, json);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occurred while trying to write to file system.");
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}