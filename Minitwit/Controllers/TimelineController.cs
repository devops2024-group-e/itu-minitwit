using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minitwit.Infrastructure;
using Minitwit.Infrastructure.Models;
using Minitwit.Utils;
using Minitwit.ViewModels;
using Minitwit.Infrastructure.Repositories;

namespace Minitwit.Controllers;

public class TimelineController : Controller
{
    private readonly ILogger<TimelineController> _logger;
    private readonly MinitwitContext _context;
    private readonly MessageRepository _messageRepository;
    private readonly UserRepository _userRepository;
    private readonly IFollowerRepository _followerRepository;

    public TimelineController(ILogger<TimelineController> logger, MinitwitContext context, MessageRepository messageRepository, UserRepository userRepository, IFollowerRepository followerRepository)
    {
        _logger = logger;
        _context = context;
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _followerRepository = followerRepository;
    }

    [Route("{username?}")]
    public IActionResult Index(string username)
    {
        bool is_loggedin = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);

        if (!string.IsNullOrEmpty(username))
        {
            var model = GetUserTimelineModel(username, is_loggedin);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        if (!is_loggedin)
        {
            return RedirectToAction("Public");
        }
        else
        {
            var model = GetCurrentUserTimelineModel(HttpContext.Session.GetInt32("user_id").Value);

            return View(model);
        }
    }

    [Route("public")]
    public IActionResult Public()
    {
        var messages = _messageRepository.GetMessages();

        return View(new PublicTimelineViewModel { Messages = messages });
    }

    [Route("{username}/follow")]
    public IActionResult FollowUser(string username)
    {
        bool is_loggedin = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
        if (!is_loggedin)
        {
            return Unauthorized();
        }

        User? profileUser = _userRepository.GetUser(username);
        if (profileUser == null)
        {
            return NotFound();
        }

        var ownUserID = HttpContext.Session.GetInt32("user_id");
        _followerRepository.AddFollower(ownUserID.Value, profileUser.UserId);

        TempData.QueueFlashMessage($"You are now following \"{profileUser.Username}\"");

        return RedirectToAction("Index", new { username });
    }

    [Route("{username}/unfollow")]
    public IActionResult UnfollowUser(string username)
    {

        bool is_loggedin = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
        if (!is_loggedin)
        {
            return Unauthorized();
        }

        User? profileUser = _userRepository.GetUser(username);
        if (profileUser == null)
        {
            return NotFound();
        }

        var ownUserID = HttpContext.Session.GetInt32("user_id");
        _followerRepository.RemoveFollower(ownUserID.Value, profileUser.UserId);

        TempData.QueueFlashMessage($"You are no longer following \"{profileUser.Username}\"");

        return RedirectToAction("Index", new { username });
    }

    [HttpPost("add_message")]
    public IActionResult AddMessage(string text)
    {
        bool is_loggedin = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
        if (!is_loggedin)
        {
            return Unauthorized();
        }
    
        var authorId = (int)HttpContext.Session.GetInt32("user_id");
        _messageRepository.AddMessage(text, authorId);

        TempData.QueueFlashMessage("Your message was recorded");

        return RedirectToAction("Index");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private TimelineViewModel? GetUserTimelineModel(string username, bool is_loggedin)
    {
        User? profileUser = _userRepository.GetUser(username);
        if (profileUser == null)
        {
            return null;
        }

        var messages = _messageRepository.GetUserSpecificMessages(profileUser);

        TimelineViewModel model = new TimelineViewModel { Messages = messages };
        model.Profile = new Profile
        {
            Username = profileUser.Username,
            UserId = (int)profileUser.UserId,
            IsMe = false,
            IsFollowing = false
        };

        if (is_loggedin)
        {
            int currentUserId = HttpContext.Session.GetInt32("user_id").Value;
            model.Profile.IsMe = currentUserId == profileUser.UserId;
            model.Profile.IsFollowing = _followerRepository.IsFollowing(currentUserId, profileUser.UserId);
            model.CurrentUsername = _userRepository.GetUser(currentUserId).Username;
        }

        //Is this really needed? Nothing has changed since the last call in line 142, as far as i can see.
        model.Messages = _messageRepository.GetUserSpecificMessages(profileUser);

        return model;
    }

    private TimelineViewModel GetCurrentUserTimelineModel(int currentUserId)
    {
        var currentUsername = _userRepository.GetUser(currentUserId).Username;

        var messages = _messageRepository.GetCurrentUserSpecificMessages(currentUserId);

        return new TimelineViewModel { CurrentUsername = currentUsername, Messages = messages };
    }
}
