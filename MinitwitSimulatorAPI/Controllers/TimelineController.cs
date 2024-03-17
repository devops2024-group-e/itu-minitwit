﻿using Microsoft.AspNetCore.Mvc;
using Minitwit.Infrastructure.Models;
using Minitwit.Infrastructure.Repositories;
using MinitwitSimulatorAPI.Models;

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
        _logger.LogDebug("Checks whether the user is logged in");
        return HttpContext.Request.Headers["Authorization"] == "Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh";
    }

    /// <summary>
    /// This method gets a user from the database by username.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <returns>A <c>User</c> object of the user with the given username.</returns>
    private User? GetUser(string username)
    {
        _logger.LogInformation($"GetUser return user with username {username}");
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
        _logger.LogDebug($"FollowUnfollowUser added latest: {latest}");

        var user = GetUser(username);
        if (user is null)
        {
            _logger.LogWarning($"FollowUnfollowUser returns NotFound for user {username}");
            return NotFound();
        }

        var ownUserId = user.UserId;
        if (!IsLoggedIn()) 
        {
            _logger.LogWarning($"FollowUnfollowUser returns Forbid because user is not logged in");
            return Forbid(); 
        }

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
        {
            _logger.LogWarning($"FollowUnfollowUser returns NotFound because other user {otherUsername} does not exist");
            return NotFound();
        }

        var otherUserId = otherUser.UserId;
        if (action == "unfollow")
        { 
            _followerRepository.RemoveFollower(ownUserId, otherUserId);
            _logger.LogInformation($"FollowUnfollowUser user {ownUserId} unfollows {otherUserId}");
        }
        else
        {
            _followerRepository.AddFollower(ownUserId, otherUserId);
            _logger.LogInformation($"FollowUnfollowUser user {ownUserId} follows {otherUserId}");
        }

        _logger.LogWarning($"FollowUnfollowUser returns NoContent");
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
        _logger.LogDebug($"AddMessage added latest: {latest}");

        if (!IsLoggedIn()) 
        { 
            _logger.LogWarning($"AddMessage returns Forbid because user {username} is not logged in");
            return Forbid(); 
        }
        string text = "";
        using (StreamReader reader = new StreamReader(HttpContext.Request.Body))
        {
            string jsonstring = await reader.ReadToEndAsync();
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonstring);
            text = dict["content"];
        }

        _messageRepository.AddMessage(text, GetUser(username).UserId);
        _logger.LogInformation($"AddMessage added message for user {username} with text {text}");

        _logger.LogWarning($"AddMessage returns NoContent");
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
        _logger.LogDebug($"GetMessages added latest: {latest}");

        User? profileUser = GetUser(username);
        if (profileUser is null)
        {
            _logger.LogWarning($"GetMessages returns NotFound for user {username}");
            return NotFound();
        }

        if (!IsLoggedIn()) 
        { 
            _logger.LogWarning($"GetMessages returns Forbid as user {username} is not logged in");
            return Forbid(); 
        }

        var messages = new List<MessageDTO>();
        foreach (var messageAuthor in _messageRepository.GetUserSpecificMessages(profileUser, no))
        {
           messages.Add(new MessageDTO(messageAuthor.Message.Text, messageAuthor.Message.PubDate.Value, messageAuthor.Author.Username));
        }
        _logger.LogInformation($"GetMessages returns all user specific messages for user {username}");
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
        _logger.LogDebug($"GetAllMessages added latest: {latest}");

        if (!IsLoggedIn()) 
        {
            _logger.LogWarning($"GetAllMessages returns Forbid because user is not logged in");
            return Forbid(); 
        }

        var messages = _messageRepository.GetMessages(no).Select(x => new MessageDTO(x.Message.Text, x.Message.PubDate.Value, x.Author.Username));

        _logger.LogInformation($"GetAllMessages returns all messages");
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
        _logger.LogDebug($"GetFollows added latest: {latest}");

        User? profileUser = GetUser(username);
        if (profileUser is null)
        {
            _logger.LogWarning($"GetFollows returns NotFound for username: {username}");
            return NotFound();
        }

        if (!IsLoggedIn()) 
        { 
            _logger.LogWarning($"GetFollows returns Forbid as user is not logged in");
            return Forbid(); 
        }

        var follows = _followerRepository.GetCurrentUserFollows(profileUser.UserId, no);

        _logger.LogInformation($"GetFollows returns followers for username {username}");
        return Ok(new FollowerDTO(follows));
    }
}
