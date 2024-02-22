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
        /// <summary>
        /// This API method removes the userid from the session. Thereby
        /// logging them out.
        /// </summary>
        /// <returns>An Http code 204 (NoContent)</returns>
        HttpContext.Session.Remove("user_id");
        return NoContent();
    }
}
