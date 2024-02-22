using Microsoft.AspNetCore.Mvc;
using MinitwitSimulatorAPI.Utils;

namespace MinitwitSimulatorAPI.Controllers;

public class LogoutController : Controller
{
    private readonly ILogger<LogoutController> _logger;

    public LogoutController(ILogger<LogoutController> logger)
    {
        _logger = logger;
    }
    
    [HttpPost("/logout")]
    public IActionResult Index()
    {
        HttpContext.Session.Remove("user_id");
        return NoContent();
    }
}
