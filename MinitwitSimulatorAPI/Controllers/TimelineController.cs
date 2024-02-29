using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinitwitSimulatorAPI.Models;
using MinitwitSimulatorAPI.Utils;
using MinitwitSimulatorAPI.ViewModels;

namespace MinitwitSimulatorAPI.Controllers;

public class TimelineController : Controller
{
    private readonly ILogger<TimelineController> _logger;
    private readonly MinitwitContext _context;

    public TimelineController(ILogger<TimelineController> logger, MinitwitContext context)
    {
        _logger = logger;
        _context = context;
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
    private User GetUser(string username)
    {
        return _context.Users.SingleOrDefault(x => x.Username == username);
    }

    /// <summary>
    /// The user currently logged in, follows or unfollows the user given in the json-body.
    /// </summary>
    /// <param name="ownUsername">The username of the user to be followed.</param>
    /// <returns>Either Http code 404 (NotFound) or Http code 204 (Nocontent)</returns>
    /// 
    [HttpPost("/fllws/{username}")]
    public async Task<IActionResult> FollowUnfollowUser([FromQuery]int latest, string username)
    {
 
        LatestDBUtils.UpdateLatest(_context, latest);

        var ownUserId = GetUser(username).UserId;
        if (!IsLoggedIn()){ return NotFound(); } // maybe should be Unauthorized();

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
        var otherUserId = GetUser(otherUsername).UserId;

        string query  = "INSERT INTO follower (who_id, whom_id) VALUES ({0}, {1})";
        if (action.Equals("unfollow")) {
            query = "DELETE FROM follower WHERE who_id = {0} AND whom_id = {1}";
        }

        _context.Database.ExecuteSqlRaw(query, ownUserId, otherUserId);
        return NoContent();
    }

    /// <summary>
    /// Adds a message to the database.
    /// </summary>
    /// <param name="username">The user whose message is to be posted</param>
    /// <returns>Either Http code 404 (NotFound) or Http code 204 (Nocontent)</returns>
    [HttpPost("/msgs/{username}")]
    public async Task<IActionResult> AddMessage([FromQuery]int latest, string username)
    {
        
        LatestDBUtils.UpdateLatest(_context, latest);

        if (!IsLoggedIn()){ return NotFound(); } // maybe should be Unauthorized();
        string text = "";
        using (StreamReader reader = new StreamReader(HttpContext.Request.Body))
        {
            string jsonstring = await reader.ReadToEndAsync();
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonstring);
            text = dict["content"];
        }

        _context.Messages.Add(new Message
        {
            AuthorId = GetUser(username).UserId,
            Text = text,
            PubDate = (int)DateTime.Now.Ticks,
            Flagged = 0
        });
        _context.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// Gets the messages posted by a given user.
    /// </summary>
    /// <param name="username">The username of the user, whose messages should be returned.</param>
    /// <returns>Either Http code 404 (NotFound) or Http code 200 (Ok)</returns>
    [HttpGet("/msgs/{username}")]
    public IActionResult GetMessages([FromQuery]int latest, string username)
    {
        LatestDBUtils.UpdateLatest(_context, latest);
       
        User? profileUser = GetUser(username);
        if (!IsLoggedIn()){ return NotFound(); } // maybe should be Unauthorized();

        var messages = (from message in _context.Messages
                        join user in _context.Users on message.AuthorId equals user.UserId
                        where user.UserId == profileUser.UserId
                        orderby message.PubDate descending
                        select new MessageAuthor { Message = message, Author = user }).Take(30).ToList();

        return Ok(messages);
    }

    /// <summary>
    /// Gets all messages in the database
    /// </summary>
    /// <returns>Http code 200 (Ok)</returns>   
    [HttpGet("/msgs")]
    public IActionResult GetAllMessages([FromQuery]int latest)
    {
        LatestDBUtils.UpdateLatest(_context, latest);
        
        var messages = (from message in _context.Messages
                        join user in _context.Users on message.AuthorId equals user.UserId
                        where message.Flagged == 0
                        orderby message.PubDate descending
                        select new MessageAuthor { Message = message, Author = user }).Take(30).ToList();

        return Ok(messages);
    }

    /// <summary>
    /// Gets the users followed by a given user.
    /// </summary>
    /// <param name="username">The username of the user, whose follows should be returned.</param>
    /// <returns>Either http code 404 (NotFound) or http code 200 (Ok)</returns>
    [HttpGet("fllws/{username}")]
    public IActionResult GetFollows([FromQuery]int latest, string username)
    {
        LatestDBUtils.UpdateLatest(_context, latest);
        
        User? profileUser = GetUser(username);
        if (!IsLoggedIn()){ return NotFound(); } // maybe should be Unauthorized();

        var follows = (from user in _context.Users
                       join follower in _context.Followers 
                       on user.UserId equals follower.WhomId
                       where follower.WhoId == profileUser.UserId
                       select user.Username).Take(100).ToList();
              
        return Ok(follows);
    }
}