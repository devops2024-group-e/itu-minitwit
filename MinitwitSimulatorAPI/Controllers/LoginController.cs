using Microsoft.AspNetCore.Mvc;
using MinitwitSimulatorAPI.Utils;
using MinitwitSimulatorAPI.ViewModels;
using MinitwitSimulatorAPI.Models;

namespace MinitwitSimulatorAPI.Controllers;
public class LoginController : Controller
{
    private readonly ILogger<LoginController> _logger;
    private readonly MinitwitContext _context;

    public LoginController(ILogger<LoginController> logger, MinitwitContext context)
    {
        _logger = logger;
        _context = context;
    }

    /* The following code is not implemented in the API yet:
    public IActionResult Index()
    {
        bool is_authenticated = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
        if (is_authenticated)
            return RedirectToAction("Index", "Timeline");

        _logger.LogInformation("Login page requested");

        return View(new LoginViewModel());
    }*/

    [HttpPost("/login")]
    public IActionResult LoginNow(string username, string password)
    {
        /// <summary>
        /// This API method logs in a user by their username and password.
        /// </summary>
        /// <param name="username">The username of the user to be logged in</param>
        /// <param name="password">The password of the user to be logged in</param>
        /// <returns>Either Http code 400 (BadRequest) or Http code 204 (Nocontent)</returns>
        _logger.LogInformation("Login attempt for user {username}", username);
        bool is_authenticated = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
        if (is_authenticated)
            //return RedirectToAction("Index", "Timeline"); // TODO: Change to '/Timeline' ??
            return NoContent();

        var user = _context.Users.SingleOrDefault(x => x.Username == username);
        if (user == null)
        {
            // NOTE: Potential security risk... not good to tell the username does not exist
            //return View("Index", new LoginViewModel() { ErrorMessage = "Invalid username" });
            return BadRequest("Invalid username");
        }

        if (PasswordHash.CheckPasswordHash(password, user.PwHash))
        {
            HttpContext.Session.SetInt32("user_id", (int)user.UserId); // TODO: This is a bad type conversion...
            //TempData.QueueFlashMessage("You were logged in");

            //return RedirectToAction("Index", "Timeline");
            return NoContent();
        }
        else
        {
            // NOTE: Potential security risk... not good to tell the password is incorrect
            //return View("Index", new LoginViewModel() { ErrorMessage = "Invalid password" });
            return BadRequest("Invalid password");
        }
    }
}