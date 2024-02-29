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
    
    /// <summary>
    /// This API method removes the userid from the session. Thereby
    /// logging them out.
    /// </summary>
    /// <returns>An Http code 204 (NoContent)</returns>
    [HttpPost("/logout")]
    public IActionResult Index()
    {
        HttpContext.Session.Remove("user_id");
        return NoContent();
    }
}
