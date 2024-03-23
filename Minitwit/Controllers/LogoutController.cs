using Microsoft.AspNetCore.Mvc;
using Minitwit.Utils;

namespace Minitwit.Controllers;

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
        _logger.LogInformation("User is logged out and redirected to timeline");
        return RedirectToAction("Index", "Timeline");
    }
}
