using System.Text.Json;
using ChainResource.service.Configuration;
using ChainResource.service.DAL.Interfaces;
using Microsoft.Extensions.Options;

namespace ChainResource.service.DAL;

public class WebServiceDal<T>: IStorageDal<T>
{
    private readonly WebServiceConfiguration _configuration;
    private readonly ILogger<WebServiceDal<T>> _logger;
    private readonly HttpClient _httpClient;

    public WebServiceDal(IOptions<WebServiceConfiguration> configuration, ILogger<WebServiceDal<T>> logger, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration.Value;
        _logger = logger;
       _httpClient = httpClientFactory.CreateClient();
    }
    public bool IsWritable { get; } = false;

    public async Task<(T value, bool IsValid)> TryReadAsync()
    {
        var fullUrl = $"{_configuration.BaseUrl}/latest.json?app_id={_configuration.ApiKey}";

        try
        {
            var response = await _httpClient.GetAsync(fullUrl);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get data from web service. Status code: {StatusCode}", response.StatusCode);
                throw new Exception($"Failed to get data from web service. Status code: {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var res = JsonSerializer.Deserialize<T>(json);
            return (res, true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occurred while trying to read from web service.");
            throw;
        }
    }

    public async Task WrtiAsync(T value)
    {
        throw new NotImplementedException();
    }
}