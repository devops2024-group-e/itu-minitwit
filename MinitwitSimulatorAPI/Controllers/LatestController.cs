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

    private async void update_latest(string request)
    {
        string parsed_command_id = "-1"
        using (StreamReader reader = new StreamReader(HttpContext.Request.Body))
        {
            string jsonstring = await reader.ReadToEndAsync();
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonstring);
            parsed_command_id = dict["latest"];
        }
        if (!parsed_command_id.equals("-1"))
        {   
            string path = "./latest_processed_sim_action_id.txt";
            using (FileStream fp = File.Open(path, FileMode.Open, FileAccess.Write, FileShare.None))
            {
                fp.Write(parsed_command_id);
            }
        }

    }

    [HttpGet("/latest")]
    public async Task<IActionResult> getLatest()
    {
        int latest_processed_command_id = -1
        string content = "";
        string path = "./latest_processed_sim_action_id.txt";

        using (FileStream fp = File.OpenRead(path))
        {
            content = fp.Read();
        }
        try
        {
            latest_processed_command_id = int(content)
        }
        return latest_processed_command_id;
    }
}