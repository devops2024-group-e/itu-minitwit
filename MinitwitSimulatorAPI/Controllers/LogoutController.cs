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

    [Route("logout")]
    public IActionResult Index()
    {
        HttpContext.Session.Remove("user_id");
        TempData.QueueFlashMessage("You were logged out");
        return RedirectToAction("Index", "Timeline");
    }
}
