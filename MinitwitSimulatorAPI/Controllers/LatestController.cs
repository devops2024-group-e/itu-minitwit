using Microsoft.AspNetCore.Mvc;
using MinitwitSimulatorAPI.Models;
using Minitwit.Infrastructure.Repositories;

namespace MinitwitSimulatorAPI.Controllers;

public class LatestController : Controller
{
    private readonly ILogger<LatestController> _logger;

    private readonly ILatestRepository _latestRepository;

    public LatestController(ILogger<LatestController> logger, ILatestRepository latestRepository)
    {
        _logger = logger;
        _latestRepository = latestRepository;
    }

    [HttpGet("/latest")]
    public async Task<ActionResult<LatestDTO>> GetLatest()
    {
        var content = await _latestRepository.GetLatestAsync();

        _logger.LogDebug($"GetLatest returns {content}");

        return Ok(new LatestDTO(content));
    }
}
