
using Microsoft.AspNetCore.Mvc;
using Minitwit;
using Minitwit.Infrastructure.Repositories;
using Minitwit.Utils;
using Minitwit.ViewModels;

[Route("Register")]
public class RegisterController : Controller
{
    private readonly ILogger<RegisterController> _logger;

    private readonly IUserRepository _userRepository;

    public RegisterController(ILogger<RegisterController> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public IActionResult Index()
    {
        bool is_authenticated = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
        if (is_authenticated)
        {
            _logger.LogDebug("User is already authenticated... redirecting to timeline");
            return RedirectToAction("Index", "Timeline");
        }

        return View(new RegisterViewModel());
    }

    [HttpPost()]
    public IActionResult Register(string username, string email, string password, string password2)
    {
        bool is_authenticated = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
        if (is_authenticated)
        {
            _logger.LogDebug("User is already authenticated... redirecting to timeline");
            return RedirectToPage("/Index"); // TODO: Change to '/Timeline' ??
        }

        string errMessage = "";

        if (string.IsNullOrEmpty(username))
            errMessage = "You have to enter a username";
        else if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            errMessage = "You have to enter a valid email address";
        else if (string.IsNullOrEmpty(password))
            errMessage = "You have to enter a password";
        else if (password != password2)
            errMessage = "The two passwords do not match";
        else if (_userRepository.DoesUserExist(username))
            errMessage = "The username is already taken";
        else
        {
            _userRepository.AddUser(username, email, PasswordHash.Hash(password));

            _logger.LogDebug("User registered: {Username}", username);
            TempData.QueueFlashMessage("You were successfully registered and can login now");

            return RedirectToAction("Index", "Login");
        }

        return View("Index", new RegisterViewModel { ErrorMessage = errMessage });
    }
}
