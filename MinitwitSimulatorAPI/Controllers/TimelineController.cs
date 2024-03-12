using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minitwit.Infrastructure;
using Minitwit.Infrastructure.Models;
using Minitwit.Infrastructure.Repositories;
using MinitwitSimulatorAPI.Models;
using MinitwitSimulatorAPI.Utils;
using MinitwitSimulatorAPI.ViewModels;

namespace MinitwitSimulatorAPI.Controllers;

public class TimelineController : Controller
{
    private readonly ILogger<TimelineController> _logger;
    private readonly ILatestRepository _latestRepository;
    private readonly IFollowerRepository _followerRepository;

    private readonly IUserRepository _userRepository;
    
    private readonly IMessageRepository _messageRepository;

    public TimelineController(ILogger<TimelineController> logger, ILatestRepository latestRepository, IFollowerRepository followerRepository, IUserRepository userRepository, IMessageRepository messageRepository)
    {
        _logger = logger;
        _latestRepository = latestRepository;
        _followerRepository = followerRepository;
        _userRepository = userRepository;
        _messageRepository = messageRepository;
    }

    /// <summary>
    /// This method checks whether the user is logged in.
    /// </summary>
    /// <returns>A <c>bool</c> describing if the user is logged in.</returns>
    private bool IsLoggedIn()
    {
        return HttpContext.Request.Headers["Authorization"] == "Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh";
    }

    /// <summary>
    /// This method gets a user from the database by username.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <returns>A <c>User</c> object of the user with the given username.</returns>
    private User? GetUser(string username)
    {
        return _userRepository.GetUser(username);
    }

    /// <summary>
    /// The user currently logged in, follows or unfollows the user given in the json-body.
    /// </summary>
    /// <param name="ownUsername">The username of the user to be followed.</param>
    /// <returns>Either Http code 404 (NotFound) or Http code 204 (Nocontent)</returns>
    ///
    [HttpPost("/fllws/{username}")]
    public async Task<IActionResult> FollowUnfollowUser([FromQuery] int latest, string username)
    {

        _latestRepository.AddLatest(latest);

        var user = GetUser(username);
        if (user is null)
            return NotFound();

        var ownUserId = user.UserId;
        if (!IsLoggedIn()) { return Forbid(); }

        string otherUsername = "";
        string action = "follow";
        using (StreamReader reader = new StreamReader(HttpContext.Request.Body))
        {
            string jsonstring = await reader.ReadToEndAsync();
            string command = jsonstring.Split(':')[0];
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonstring);
            if (command.Contains("unfollow"))
            {
                action = "unfollow";
            }
            otherUsername = dict[action];
        }
        var otherUser = GetUser(otherUsername);

        if (otherUser is null)
            return NotFound();

        if (action == "unfollow")
            _followerRepository.RemoveFollower(ownUserId, otherUser.UserId);
        else
            _followerRepository.AddFollower(ownUserId, otherUser.UserId);

        return NoContent();
    }

    /// <summary>
    /// Adds a message to the database.
    /// </summary>
    /// <param name="username">The user whose message is to be posted</param>
    /// <returns>Either Http code 404 (NotFound) or Http code 204 (Nocontent)</returns>
    [HttpPost("/msgs/{username}")]
    public async Task<IActionResult> AddMessage([FromQuery] int latest, string username)
    {

        _latestRepository.AddLatest(latest);

        if (!IsLoggedIn()) { return Forbid(); }
        string text = "";
        using (StreamReader reader = new StreamReader(HttpContext.Request.Body))
        {
            string jsonstring = await reader.ReadToEndAsync();
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonstring);
            text = dict["content"];
        }

        _messageRepository.AddMessage(text, GetUser(username).UserId);

        return NoContent();
    }

    /// <summary>
    /// Gets the messages posted by a given user.
    /// </summary>
    /// <param name="username">The username of the user, whose messages should be returned.</param>
    /// <returns>Either Http code 404 (NotFound) or Http code 200 (Ok)</returns>
    [HttpGet("/msgs/{username}")]
    public IActionResult GetMessages([FromQuery] int latest, string username, [FromQuery] int no = 100)
    {
        _latestRepository.AddLatest(latest);

        User? profileUser = GetUser(username);
        if (profileUser is null)
            return NotFound();

        if (!IsLoggedIn()) { return Forbid(); }

        var messages = new List<MessageDTO>();
        foreach (var messageAuthor in _messageRepository.GetUserSpecificMessages(profileUser, no))
        {
           messages.Add(new MessageDTO(messageAuthor.Message.Text, messageAuthor.Message.PubDate.Value, messageAuthor.Author.Username));
        }

        return Ok(messages);
    }

    /// <summary>
    /// Gets all messages in the database
    /// </summary>
    /// <returns>Http code 200 (Ok)</returns>
    [HttpGet("/msgs")]
    public IActionResult GetAllMessages([FromQuery] int latest, [FromQuery] int no = 100)
    {
        _latestRepository.AddLatest(latest);

        if (!IsLoggedIn()) { return Forbid(); }

        var messages = _messageRepository.GetMessages(no).Select(x => new MessageDTO(x.Message.Text, x.Message.PubDate.Value, x.Author.Username));

        return Ok(messages);
    }

    /// <summary>
    /// Gets the users followed by a given user.
    /// </summary>
    /// <param name="username">The username of the user, whose follows should be returned.</param>
    /// <returns>Either http code 404 (NotFound) or http code 200 (Ok)</returns>
    [HttpGet("fllws/{username}")]
    public IActionResult GetFollows([FromQuery] int latest, string username, [FromQuery] int no = 100)
    {
        _latestRepository.AddLatest(latest);

        User? profileUser = GetUser(username);
        if (profileUser is null)
            return NotFound();

        if (!IsLoggedIn()) { return Forbid(); }

        var follows = _followerRepository.GetCurrentUserFollows(profileUser.UserId, no);

        return Ok(new FollowerDTO(follows));
    }
}
