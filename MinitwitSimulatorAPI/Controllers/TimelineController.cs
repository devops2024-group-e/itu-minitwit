using Microsoft.AspNetCore.Mvc;
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
        return HttpContext.Request.Headers["Authorization"] == "Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh";
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

        await _latestRepository.AddLatestAsync(latest);

        var user = await _userRepository.GetUserAsync(username);
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
        var otherUser = await _userRepository.GetUserAsync(otherUsername);

        if (otherUser is null)
            return NotFound();

        if (action == "unfollow")
            await _followerRepository.RemoveFollowerAsync(ownUserId, otherUser.UserId);
        else
            await _followerRepository.AddFollowerAsync(ownUserId, otherUser.UserId);

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

        await _latestRepository.AddLatestAsync(latest);

        if (!IsLoggedIn()) { return Forbid(); }
        string text = "";
        using (StreamReader reader = new StreamReader(HttpContext.Request.Body))
        {
            string jsonstring = await reader.ReadToEndAsync();
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonstring);
            text = dict["content"];
        }

        var user = await _userRepository.GetUserAsync(username);
        if (user is null)
            return NotFound();

        _messageRepository.AddMessage(text, user.UserId);

        return NoContent();
    }

    /// <summary>
    /// Gets the messages posted by a given user.
    /// </summary>
    /// <param name="username">The username of the user, whose messages should be returned.</param>
    /// <returns>Either Http code 404 (NotFound) or Http code 200 (Ok)</returns>
    [HttpGet("/msgs/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessages([FromQuery] int latest, string username, [FromQuery] int no = 100)
    {
        await _latestRepository.AddLatestAsync(latest);

        User? profileUser = await _userRepository.GetUserAsync(username);
        if (profileUser is null)
            return NotFound();

        if (!IsLoggedIn()) { return Forbid(); }

        var messages = (await _messageRepository.GetUserSpecificMessagesAsync(profileUser, no))
                        .Select(messageAuthor => new MessageDTO(messageAuthor.Message.Text, messageAuthor.Message.PubDate.GetValueOrDefault(0), messageAuthor.Author.Username));

        return Ok(messages);
    }

    /// <summary>
    /// Gets all messages in the database
    /// </summary>
    /// <returns>Http code 200 (Ok)</returns>
    [HttpGet("/msgs")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetAllMessages([FromQuery] int latest, [FromQuery] int no = 100)
    {
        await _latestRepository.AddLatestAsync(latest);

        if (!IsLoggedIn()) { return Forbid(); }

        var messages = (await _messageRepository.GetMessagesAsync(no)).Select(x => new MessageDTO(x.Message.Text, x.Message.PubDate.GetValueOrDefault(0), x.Author.Username));

        return Ok(messages);
    }

    /// <summary>
    /// Gets the users followed by a given user.
    /// </summary>
    /// <param name="username">The username of the user, whose follows should be returned.</param>
    /// <returns>Either http code 404 (NotFound) or http code 200 (Ok)</returns>
    [HttpGet("fllws/{username}")]
    public async Task<ActionResult<FollowerDTO>> GetFollows([FromQuery] int latest, string username, [FromQuery] int no = 100)
    {
        await _latestRepository.AddLatestAsync(latest);

        User? profileUser = await _userRepository.GetUserAsync(username);
        if (profileUser is null)
            return NotFound();

        if (!IsLoggedIn()) { return Forbid(); }

        var follows = await _followerRepository.GetCurrentUserFollowsAsync(profileUser.UserId, no);

        return Ok(new FollowerDTO(follows));
    }
}
