using ChainResource.service.BL.Interfaces;
using ChainResource.service.Model;
using Microsoft.AspNetCore.Mvc;

namespace ChainResource.service.Controllers;

[ApiController]               
[Route("api/[controller]")]      
public class RatesController : ControllerBase
{
    private readonly  ILogger<RatesController> _logger;
    private readonly IChainResource<ExchangeRateList> _chainResource;

    public RatesController(ILogger<RatesController> logger, IChainResource<ExchangeRateList> chainResource)
    {
        _logger = logger;
        _chainResource = chainResource;
    }
    [HttpGet]
    public async Task<ActionResult<ExchangeRateList>> GetRates()
    {
        try
        {
            var rates = await _chainResource.GetValue();
            return Ok(rates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving exchange rates");
            return StatusCode(500, "Internal server error");
        }

    }
}