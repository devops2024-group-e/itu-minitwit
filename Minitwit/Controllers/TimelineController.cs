using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Minitwit.Models;
using Minitwit.Utils;
using Minitwit.ViewModels;

namespace Minitwit.Controllers;

public class TimelineController : Controller
{
    private readonly ILogger<TimelineController> _logger;
    private readonly MinitwitContext _context;

    public TimelineController(ILogger<TimelineController> logger, MinitwitContext context)
    {
        _logger = logger;
        _context = context;
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
            var model = GetCurrentUserTimelineModel((int)HttpContext.Session.GetInt32("user_id"));

            return View(model);
        }
    }

    [Route("public")]
    public IActionResult Public()
    {
        var messages = (from message in _context.Messages
                        join user in _context.Users on message.AuthorId equals user.UserId
                        where message.Flagged == 0
                        orderby message.PubDate descending
                        select new MessageAuthor { Message = message, Author = user }).Take(30).ToList();

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

        User? profileUser = _context.Users.SingleOrDefault(x => x.Username == username);
        if (profileUser == null)
        {
            return NotFound();
        }

        var ownUserID = HttpContext.Session.GetInt32("user_id");
        _context.Followers.Add(new Follower { WhoId = ownUserID, WhomId = profileUser.UserId });
        _context.SaveChanges();

        TempData.QueueFlashMessage($"You are now following {profileUser.Username}");

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

        User? profileUser = _context.Users.SingleOrDefault(x => x.Username == username);
        if (profileUser == null)
        {
            return NotFound();
        }

        var ownUserID = HttpContext.Session.GetInt32("user_id");
        _context.Followers.Remove(_context.Followers.Single(x => x.WhoId == ownUserID && x.WhomId == profileUser.UserId));
        _context.SaveChanges();

        TempData.QueueFlashMessage($"You are no longer following {profileUser.Username}");

        return RedirectToAction("Index", new { username });
    }

    [HttpPost("add_message")]
    public IActionResult AddMessage(string message)
    {
        bool is_loggedin = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
        if (!is_loggedin)
        {
            return Unauthorized();
        }

        _context.Messages.Add(new Message
        {
            AuthorId = (int)HttpContext.Session.GetInt32("user_id"),
            Text = message,
            PubDate = DateTime.Now.Ticks,
            Flagged = 0
        });
        _context.SaveChanges();

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
        User? profileUser = _context.Users.SingleOrDefault(x => x.Username == username);
        if (profileUser == null)
        {
            return null;
        }

        var messages = (from message in _context.Messages
                        join user in _context.Users on message.AuthorId equals user.UserId
                        where user.UserId == profileUser.UserId
                        orderby message.PubDate descending
                        select new MessageAuthor { Message = message, Author = user }).Take(30).ToList();

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
            int currentUserId = (int)HttpContext.Session.GetInt32("user_id");
            model.Profile.IsMe = currentUserId == profileUser.UserId;
            model.Profile.IsFollowing = _context.Followers
                                .Any(x => x.WhoId == currentUserId && x.WhomId == profileUser.UserId);

            model.CurrentUsername = _context.Users.Single(x => x.UserId == currentUserId).Username;
        }

        model.Messages = (from message in _context.Messages
                          join user in _context.Users on message.AuthorId equals user.UserId
                          where user.UserId == profileUser.UserId
                          orderby message.PubDate descending
                          select new MessageAuthor { Message = message, Author = user }).Take(30).ToList();

        return model;
    }

    private TimelineViewModel GetCurrentUserTimelineModel(int currentUserId)
    {
        var currentUsername = _context.Users.Single(x => x.UserId == currentUserId).Username;

        var messages = (from user in _context.Users
                        join msg in _context.Messages on user.UserId equals msg.AuthorId
                        where msg.Flagged == 0 && (user.UserId == currentUserId || (from f in _context.Followers
                                                                                    where f.WhoId == currentUserId
                                                                                    select f.WhomId).Contains(currentUserId))
                        orderby msg.PubDate descending
                        select new MessageAuthor { Author = user, Message = msg }).Take(30).ToList();

        return new TimelineViewModel { CurrentUsername = currentUsername, Messages = messages };
    }
}
