using ClickMe.Application.Services.Counters;
using ClickMe.Application.Services.Counters.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ClickMe.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CounterController : Controller {
    private readonly ICounterService _counterService;
    private readonly ILogger<CounterController> _logger;

    public CounterController(ICounterService counterService, ILogger<CounterController> logger) {
        _counterService = counterService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<CounterDto>>> GetAllAsync() {
        _logger.LogInformation("Getting all counters");

        try {
            List<CounterDto> counters = await _counterService.GetAllAsync();
            _logger.LogInformation("Returning all counters");
            return Ok(counters);
        } catch (Exception ex) {
            _logger.LogError(ex, "Error getting all counters");
            return BadRequest();
        }
    }
}
