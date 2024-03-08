
using Microsoft.AspNetCore.Mvc;
using Minitwit.Infrastructure;
using Minitwit.Infrastructure.Repositories;
using MinitwitSimulatorAPI;
using MinitwitSimulatorAPI.Models;
using MinitwitSimulatorAPI.Utils;
using MinitwitSimulatorAPI.ViewModels;
public class RegisterController : Controller
{
    private readonly ILogger<RegisterController> _logger;
    private readonly MinitwitContext _context;

    private readonly ILatestRepository _latestRepository;

    public RegisterController(ILogger<RegisterController> logger, MinitwitContext context, ILatestRepository latestRepository)
    {
        _logger = logger;
        _context = context;
        _latestRepository = latestRepository;
    }

    /// <summary>
    /// This API method inserts a user into the database.
    /// </summary>
    /// <param name="username">The username of the new user.</param>
    /// <param name="email">The email of the new user.</param>
    /// <param name="password">The password of the new user.</param>
    /// <param name="password2">A repetition of the password of the new user.</param>
    /// <returns>Either http code 400 (BadRequest) or http code 204 (Nocontent)</returns>
    [HttpPost("/register")]
    public IActionResult Register([FromQuery] int latest, [FromBody] RegisterUser user)
    {
        LatestDBUtils.UpdateLatest(_latestRepository, latest);

        string errMessage = "";

        if (string.IsNullOrEmpty(user.Username))
            errMessage = "You have to enter a username";
        else if (string.IsNullOrEmpty(user.Email) || !user.Email.Contains("@"))
            errMessage = "You have to enter a valid email address";
        else if (string.IsNullOrEmpty(user.Pwd))
            errMessage = "You have to enter a password";
        else if (_context.Users.Any(x => x.Username == user.Username))
            errMessage = "The username is already taken";
        else
        {
            User userRecord = new User // We should use another type to represent the model the database
            {
                Username = user.Username,
                Email = user.Email,
                PwHash = PasswordHash.Hash(user.Pwd)
            };

            _context.Users.Add(userRecord);
            _context.SaveChanges();

            _logger.LogDebug("User registered: {Username}", user.Username);
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
