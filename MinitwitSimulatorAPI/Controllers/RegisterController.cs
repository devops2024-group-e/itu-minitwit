using Microsoft.AspNetCore.Mvc;
using Minitwit.Infrastructure.Repositories;
using MinitwitSimulatorAPI.Models;
using MinitwitSimulatorAPI.Utils;

namespace MinitwitSimulatorAPI.Controllers;

public class RegisterController : Controller
{
    private readonly ILogger<RegisterController> _logger;

    private readonly ILatestRepository _latestRepository;
    private readonly IUserRepository _userRepository;

    public RegisterController(ILogger<RegisterController> logger, ILatestRepository latestRepository, IUserRepository userRepository)
    {
        _logger = logger;
        _latestRepository = latestRepository;
        _userRepository = userRepository;
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
        _latestRepository.AddLatest(latest);
        _logger.LogDebug($"Register added latest: {latest}");

        string errMessage = "";

        if (string.IsNullOrEmpty(user.Username))
        {
            errMessage = "You have to enter a username";
            _logger.LogWarning($"Username in register is empty");
        }
        else if (string.IsNullOrEmpty(user.Email) || !user.Email.Contains("@"))
        {
            errMessage = "You have to enter a valid email address";
            _logger.LogWarning($"Email in register is not valid");
        }
        else if (string.IsNullOrEmpty(user.Pwd))
        {
            errMessage = "You have to enter a password";
            _logger.LogWarning($"Password in register is empty");

        }
        else if (_userRepository.DoesUserExist(user.Username))
        {
            errMessage = "The username is already taken";
            _logger.LogWarning($"Username provided for register already exists");
        }
        else
        {
            _userRepository.AddUser(user.Username, user.Email, PasswordHash.Hash(user.Pwd));
            _logger.LogInformation("User registered: {Username}", user.Username);
        }
        if (errMessage != "")
        {
            _logger.LogDebug("Register returns BadRequest");
            return BadRequest(errMessage);
        }
        else
        {
            _logger.LogDebug("Register returns 204 NoContent");
            return NoContent();
        }
    }
}
