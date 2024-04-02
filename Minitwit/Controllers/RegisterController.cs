using Microsoft.AspNetCore.Mvc;
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
        if (HttpContext.Session.IsAuthenticated())
        {
            _logger.LogInformation("User is already authenticated. Redirecting to timeline");
            return RedirectToAction("Index", "Timeline");
        }

        return View(new RegisterViewModel());
    }

    [HttpPost()]
    public async Task<IActionResult> Register(string username, string email, string password, string password2)
    {
        if (HttpContext.Session.IsAuthenticated())
        {
            _logger.LogInformation("User is already authenticated. Redirecting to timeline");
            return RedirectToPage("/Index"); // TODO: Change to '/Timeline' ??
        }

        string errMessage;

        if (string.IsNullOrEmpty(username))
        {
            errMessage = "You have to enter a username";
            _logger.LogWarning($"Username in register is empty");
        }
        else if (string.IsNullOrEmpty(email) || !email.Contains("@"))
        {
            errMessage = "You have to enter a valid email address";
            _logger.LogWarning($"Email in register is not valid");
        }
        else if (string.IsNullOrEmpty(password))
        {
            errMessage = "You have to enter a password";
            _logger.LogWarning($"Password in register is empty");
        }
        else if (password != password2)
        {
            errMessage = "The two passwords do not match";
            _logger.LogWarning($"Two passwords given to register do not match");
        }
        else if (await _userRepository.DoesUserExistAsync(username))
        {
            errMessage = "The username is already taken";
            _logger.LogWarning($"Username provided for register already exists");
        }
        else
        {
            await _userRepository.AddUserAsync(username, email, PasswordHash.Hash(password));

            _logger.LogInformation("User registered: {Username}", username);
            TempData.QueueFlashMessage("You were successfully registered and can login now");

            return RedirectToAction("Index", "Login");
        }

        return View("Index", new RegisterViewModel { ErrorMessage = errMessage });
    }
}
