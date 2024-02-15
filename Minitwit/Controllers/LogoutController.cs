using Microsoft.AspNetCore.Mvc;

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
        return RedirectToAction("Index", "Timeline");
    }
}
