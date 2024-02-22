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

    private bool IsLoggedIn()
    {
        /// <summary>
        /// This method checks whether the user is logged in.
        /// </summary>
        /// <returns>A <c>bool</c> describing if the user is logged in.</returns>

        return HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
    }

    private User GetUser(string username)
    {
        /// <summary>
        /// This method gets a user from the database by username.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>A <c>User</c> object of the user with the given username.</returns>

        return _context.Users.SingleOrDefault(x => x.Username == username);
    }

    private int GetCurrentUserId()
    {
        /// <summary>
        /// This method gets the <c>UserId</c> from the user currently logged in to the session.
        /// </summary>
        /// <returns>The UserId as an <c>int</c>.</returns>

        return (int)HttpContext.Session.GetInt32("user_id");
    }

    [HttpPost("/fllws/{username}/follow")]
    public IActionResult FollowUser(string username)
    {
        /// <summary>
        /// The user currently logged in, follows the given user.
        /// </summary>
        /// <param name="username">The username of the user to be followed.</param>
        /// <returns>Either Http code 404 (NotFound) or Http code 204 (Nocontent)</returns>
        
        User? profileUser = GetUser(username);
        if (!IsLoggedIn()){ return NotFound(); } // maybe should be Unauthorized();


        var ownUserID = HttpContext.Session.GetInt32("user_id");
        _context.Database.ExecuteSqlRaw("INSERT INTO follower (who_id, whom_id) VALUES ({0}, {1})", ownUserID, profileUser.UserId);

        return NoContent();
    }


    [HttpPost("/fllws/{username}/unfollow")]
    public IActionResult UnfollowUser(string username)
    {
        /// <summary>
        /// The user currently logged in, unfollows the given user.
        /// </summary>
        /// <param name="username">The username of the user to be unfollowed.</param>
        /// <returns>Either Http code 404 (NotFound) or Http code 204 (Nocontent)</returns>
        
        User? profileUser = GetUser(username);
        if (!IsLoggedIn()){ return NotFound(); } // maybe should be Unauthorized();

        var ownUserID = HttpContext.Session.GetInt32("user_id");
        _context.Database.ExecuteSqlRaw("DELETE FROM follower WHERE who_id = {0} AND whom_id = {1}", ownUserID, profileUser.UserId);

        return NoContent();
    }

    [HttpPost("/msgs/{username}")]
    public IActionResult AddMessage(string text)
    {
        /// <summary>
        /// Adds a message to the database.
        /// </summary>
        /// <param name="text">The message to be posted.</param>
        /// <returns>Either Http code 404 (NotFound) or Http code 204 (Nocontent)</returns>
        
        if (!IsLoggedIn()){ return NotFound(); } // maybe should be Unauthorized();

        _context.Messages.Add(new Message
        {
            AuthorId = (int)HttpContext.Session.GetInt32("user_id"),
            Text = text,
            PubDate = DateTime.Now.Ticks,
            Flagged = 0
        });
        _context.SaveChanges();

        return NoContent();
    }

    [HttpGet("/msgs/{username}")]
    public IActionResult GetMessages(string username)
    {
        /// <summary>
        /// Gets the messages posted by a given user.
        /// </summary>
        /// <param name="username">The username of the user, whose messages should be returned.</param>
        /// <returns>Either Http code 404 (NotFound) or Http code 200 (Ok)</returns>
        
        User? profileUser = GetUser(username);
        if (!IsLoggedIn()){ return NotFound(); } // maybe should be Unauthorized();

        var messages = (from message in _context.Messages
                        join user in _context.Users on message.AuthorId equals user.UserId
                        where user.UserId == profileUser.UserId
                        orderby message.PubDate descending
                        select new MessageAuthor { Message = message, Author = user }).Take(30).ToList();

        return Ok(messages);
    }

    [HttpGet("/msgs")]
    public IActionResult GetAllMessages()
    {
        /// <summary>
        /// Gets all messages in the database
        /// </summary>
        /// <returns>Http code 200 (Ok)</returns>
        var messages = (from message in _context.Messages
                        join user in _context.Users on message.AuthorId equals user.UserId
                        where message.Flagged == 0
                        orderby message.PubDate descending
                        select new MessageAuthor { Message = message, Author = user }).Take(30).ToList();

        return Ok(messages);
    }

    [HttpGet("fllws/{username}")]
    public IActionResult GetFollows(string username)
    {
        /// <summary>
        /// Gets the users followed by a given user.
        /// </summary>
        /// <param name="username">The username of the user, whose follows should be returned.</param>
        /// <returns>Either http code 404 (NotFound) or http code 200 (Ok)</returns>
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