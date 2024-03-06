﻿using System.Diagnostics;
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
    private User? GetUser(string username)
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
    public async Task<IActionResult> FollowUnfollowUser([FromQuery] int latest, string username)
    {

        LatestDBUtils.UpdateLatest(_context, latest);

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
            _context.Followers.Remove(new Follower { WhoId = ownUserId, WhomId = otherUser.UserId });
        else
            _context.Followers.Add(new Follower { WhoId = ownUserId, WhomId = otherUser.UserId });

        _context.SaveChanges();

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

        LatestDBUtils.UpdateLatest(_context, latest);

        if (!IsLoggedIn()) { return Forbid(); }
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
    public IActionResult GetMessages([FromQuery] int latest, string username, [FromQuery] int no = 100)
    {
        LatestDBUtils.UpdateLatest(_context, latest);

        User? profileUser = GetUser(username);
        if (profileUser is null)
            return NotFound();

        if (!IsLoggedIn()) { return Forbid(); }

        var messages = (from message in _context.Messages
                        join user in _context.Users on message.AuthorId equals user.UserId
                        where user.UserId == profileUser.UserId
                        orderby message.PubDate descending
                        select new { Content = message.Text, message.PubDate, User = user.Username }).Take(no).ToList();

        return Ok(messages);
    }

    /// <summary>
    /// Gets all messages in the database
    /// </summary>
    /// <returns>Http code 200 (Ok)</returns>
    [HttpGet("/msgs")]
    public IActionResult GetAllMessages([FromQuery] int latest, [FromQuery] int no = 100)
    {
        LatestDBUtils.UpdateLatest(_context, latest);

        if (!IsLoggedIn()) { return Forbid(); }

        var messages = (from message in _context.Messages
                        join user in _context.Users on message.AuthorId equals user.UserId
                        where message.Flagged == 0
                        orderby message.PubDate descending
                        select new { Content = message.Text, message.PubDate, User = user.Username }).Take(no).ToList();

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
        LatestDBUtils.UpdateLatest(_context, latest);

        User? profileUser = GetUser(username);
        if (profileUser is null)
            return NotFound();

        if (!IsLoggedIn()) { return Forbid(); }

        var follows = (from user in _context.Users
                       join follower in _context.Followers
                       on user.UserId equals follower.WhomId
                       where follower.WhoId == profileUser.UserId
                       select user.Username).Take(no).ToList();

        return Ok(new { Follows = follows });
    }
}
