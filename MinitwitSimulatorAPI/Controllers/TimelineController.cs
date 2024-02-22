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
        return HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
    }

    private User GetUser(string username)
    {
        return _context.Users.SingleOrDefault(x => x.Username == username);
    }

    private int GetCurrentUserId()
    {
        return (int)HttpContext.Session.GetInt32("user_id");
    }

    [HttpPost("{username}/follow")]
    public IActionResult FollowUser(string username)
    {
        User? profileUser = GetUser(username);
        if (!IsLoggedIn()){ return NotFound(); } // maybe should be Unauthorized();


        var ownUserID = HttpContext.Session.GetInt32("user_id");
        _context.Database.ExecuteSqlRaw("INSERT INTO follower (who_id, whom_id) VALUES ({0}, {1})", ownUserID, profileUser.UserId);

        return NoContent();
    }


    [HttpPost("{username}/unfollow")]
    public IActionResult UnfollowUser(string username)
    {
        User? profileUser = GetUser(username);
        if (!IsLoggedIn()){ return NotFound(); } // maybe should be Unauthorized();

        var ownUserID = HttpContext.Session.GetInt32("user_id");
        _context.Database.ExecuteSqlRaw("DELETE FROM follower WHERE who_id = {0} AND whom_id = {1}", ownUserID, profileUser.UserId);

        return NoContent();
    }

    [HttpPost("add_message")]
    public IActionResult AddMessage(string text)
    {
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

    [HttpGet("{username}/messages")]
    public IActionResult GetMessages(string username)
    {
        User? profileUser = GetUser(username);
        if (!IsLoggedIn()){ return NotFound(); } // maybe should be Unauthorized();

        var messages = (from message in _context.Messages
                        join user in _context.Users on message.AuthorId equals user.UserId
                        where user.UserId == profileUser.UserId
                        orderby message.PubDate descending
                        select new MessageAuthor { Message = message, Author = user }).Take(30).ToList();

        return Ok(messages);
    }

    [HttpGet("fllws/{username}")]
    public IActionResult GetFollows(string username)
    /// <summary>
    /// This API method returns the users whom the current logged in user follows.
    /// </summary>
    {
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