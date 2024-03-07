using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinitwitSimulatorAPI.Models;
using MinitwitSimulatorAPI.Utils;
using MinitwitSimulatorAPI.ViewModels;
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
    public IActionResult GetLatest()
    {
        var content = _latestRepository.GetLatest();

        return Ok(new LatestDTO(content));
    }
}
