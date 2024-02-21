
using Microsoft.AspNetCore.Mvc;
using MinitwitSimulatorAPI;
using MinitwitSimulatorAPI.Models;
using MinitwitSimulatorAPI.Utils;
using MinitwitSimulatorAPI.ViewModels;

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

    /* The following code is not implemented in the API yet:
    public IActionResult Index()
    {
        bool is_authenticated = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
        if (is_authenticated)
        {
            _logger.LogDebug("User is already authenticated... redirecting to timeline");
            return RedirectToAction("Index", "Timeline");
        }

        return View(new RegisterViewModel());
    }*/

    [HttpPost]
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
            //TempData.QueueFlashMessage("You were successfully registered and can login now");
        }
        if (errMessage != "")
        {
            return BadRequest(errMessage);
        }
        else
        {
            return NoContent();
        }
    }
}
