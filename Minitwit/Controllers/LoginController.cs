using Microsoft.AspNetCore.Mvc;
using Minitwit.Infrastructure.Repositories;
using Minitwit.Utils;
using Minitwit.ViewModels;

namespace Minitwit.Controllers;

[Route("Login")]
public class LoginController : Controller
{
    private readonly ILogger<LoginController> _logger;

    private readonly IUserRepository _userRepository;

    public LoginController(ILogger<LoginController> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public IActionResult Index()
    {
        bool is_authenticated = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
        if (is_authenticated)
        {
            _logger.LogDebug("User is already logged in and redirected to timeline");
            return RedirectToAction("Index", "Timeline");
        }

        _logger.LogInformation("Login page requested");

        return View(new LoginViewModel());
    }

    [HttpPost()]
    public IActionResult LoginNow(string username, string password)
    {
        _logger.LogInformation("Login attempt for user {username}", username);
        bool is_authenticated = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
        if (is_authenticated)
        {
            _logger.LogDebug("User is already logged in and redirected to timeline");
            return RedirectToAction("Index", "Timeline"); // TODO: Change to '/Timeline' ??
        }

        var user = _userRepository.GetUser(username);
        if (user == null)
        {
            // NOTE: Potential security risk... not good to tell the username does not exist
            _logger.LogWarning($"User {username} does not exist");
            return View("Index", new LoginViewModel() { ErrorMessage = "Invalid username" });
        }

        if (PasswordHash.CheckPasswordHash(password, user.PwHash))
        {
            HttpContext.Session.SetInt32("user_id", (int)user.UserId); // TODO: This is a bad type conversion...
            TempData.QueueFlashMessage("You were logged in");

            _logger.LogInformation($"User with username {username} was logged in and redirected to Timeline");

            return RedirectToAction("Index", "Timeline");
        }
        else
        {
            // NOTE: Potential security risk... not good to tell the password is incorrect
            _logger.LogInformation($"User with username {username} provided wrong password and was redirected to Login page");
            return View("Index", new LoginViewModel() { ErrorMessage = "Invalid password" });
        }
    }
}
