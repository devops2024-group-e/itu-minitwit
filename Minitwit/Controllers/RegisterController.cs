
using Microsoft.AspNetCore.Mvc;
using Minitwit;
using Minitwit.Models;
using Minitwit.Utils;
using Minitwit.ViewModels;

[Route("Register")]
public class RegisterController : Controller
{
    private readonly ILogger<RegisterController> _logger;
    private readonly MinitwitContext _context;

    public RegisterController(ILogger<RegisterController> logger, MinitwitContext context)
    {
        _logger = logger;
        _context = context;
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
            errMessage = "Username is required";
        else if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            errMessage = "You have to enter a valid email address";
        else if (string.IsNullOrEmpty(password))
            errMessage = "Password is required";
        else if (password != password2)
            errMessage = "The two passwords do not match";
        else if (_context.Users.Any(x => x.Username == username))
            errMessage = "The username is already taken";
        else
        {
            User user = new User // We should use another type to represent the model the database
            {
                Username = username,
                Email = email,
                PwHash = PasswordHash.Hash(password)
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            _logger.LogDebug("User registered: {Username}", username);
            TempData.QueueFlashMessage("You were successfully registered and can login now");

            return RedirectToAction("Index", "Login");
        }

        return View("Index", new RegisterViewModel { ErrorMessage = errMessage });
    }
}
