using System;  
using System.IO;  
using System.Text;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinitwitSimulatorAPI.Models;
using MinitwitSimulatorAPI.Utils;
using MinitwitSimulatorAPI.ViewModels;

namespace MinitwitSimulatorAPI.Controllers;

public class LatestController : Controller
{
    private readonly ILogger<LatestController> _logger;

    private readonly MinitwitContext _context;

    public LatestController(ILogger<LatestController> logger, MinitwitContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("/latest")]
    public IActionResult GetLatest()
    {
        var content = _context.Latests.MaxBy(x => x.CommandId);
        int latest_processed_command_id = content?.CommandId ?? -1;
        return Ok(new {Latest = latest_processed_command_id});
    }
}