
using Microsoft.AspNetCore.Mvc;
using MinitwitSimulatorAPI;
using MinitwitSimulatorAPI.Models;
using MinitwitSimulatorAPI.Utils;
using MinitwitSimulatorAPI.ViewModels;
public class RegisterController : Controller
{
    private readonly ILogger<RegisterController> _logger;
    private readonly MinitwitContext _context;

    public RegisterController(ILogger<RegisterController> logger, MinitwitContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost("/register")]
    public IActionResult Register(string username, string email, string password, string password2)
    {
        /// <summary>
        /// This API method inserts a user into the database.
        /// </summary>
        /// <param name="username">The username of the new user.</param>
        /// <param name="email">The email of the new user.</param>
        /// <param name="password">The password of the new user.</param>
        /// <param name="password2">A repetition of the password of the new user.</param>
        /// <returns>Either http code 400 (BadRequest) or http code 204 (Nocontent)</returns>

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
