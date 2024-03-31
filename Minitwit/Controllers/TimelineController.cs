using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Minitwit.Infrastructure.Models;
using Minitwit.Utils;
using Minitwit.ViewModels;
using Minitwit.Infrastructure.Repositories;

namespace Minitwit.Controllers;

public class TimelineController : Controller
{
    private readonly ILogger<TimelineController> _logger;
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFollowerRepository _followerRepository;

    public TimelineController(ILogger<TimelineController> logger, IMessageRepository messageRepository, IUserRepository userRepository, IFollowerRepository followerRepository)
    {
        _logger = logger;
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _followerRepository = followerRepository;
    }

    [Route("{username?}")]
    public async Task<IActionResult> Index(string username)
    {
        bool is_loggedin = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);

        if (!string.IsNullOrEmpty(username))
        {
            var model = await GetUserTimelineModelAsync(username, is_loggedin);
            if (model == null)
            {
                _logger.LogWarning("Index in Timeline returns NotFound");
                return NotFound();
            }

            return View(model);
        }

        if (!is_loggedin)
        {
            _logger.LogInformation($"Redirecting to public timeline as user {username} is not logged in");
            return RedirectToAction("Public");
        }
        else
        {
            var model = await GetCurrentUserTimelineModelAsync(HttpContext.Session.GetInt32("user_id").Value);

            _logger.LogInformation($"Redirecting to timeline for user {username}");
            return View(model);
        }
    }

    [Route("public")]
    public async Task<IActionResult> Public()
    {
        var messages = await _messageRepository.GetMessagesAsync(30);

        _logger.LogInformation($"Redirecting to public timeline");
        return View(new PublicTimelineViewModel { Messages = messages });
    }

    [Route("{username}/follow")]
    public async Task<IActionResult> FollowUser(string username)
    {
        bool is_loggedin = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
        if (!is_loggedin)
        {
            _logger.LogWarning($"FollowUser returns Unauthorized because user {username} is not logged in");
            return Unauthorized();
        }

        User? profileUser = await _userRepository.GetUserAsync(username);
        if (profileUser == null)
        {
            _logger.LogWarning($"FollowUser returns NotFound for user {username}");
            return NotFound();
        }

        var ownUserID = HttpContext.Session.GetInt32("user_id");
        await _followerRepository.AddFollowerAsync(ownUserID.Value, profileUser.UserId);
        _logger.LogInformation($"FollowUser user {username} now follows {profileUser.Username}");

        TempData.QueueFlashMessage($"You are now following \"{profileUser.Username}\"");

        return RedirectToAction("Index", new { username });
    }

    [Route("{username}/unfollow")]
    public async Task<IActionResult> UnfollowUser(string username)
    {

        bool is_loggedin = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
        if (!is_loggedin)
        {
            _logger.LogWarning($"UnfollowUser returns Unauthorized because user {username} is not logged in");
            return Unauthorized();
        }

        User? profileUser = await _userRepository.GetUserAsync(username);
        if (profileUser == null)
        {
            _logger.LogWarning($"UnfollowUser returns NotFound for user {username}");
            return NotFound();
        }

        var ownUserID = HttpContext.Session.GetInt32("user_id");
        await _followerRepository.RemoveFollowerAsync(ownUserID.Value, profileUser.UserId);
        _logger.LogInformation($"UnfollowUser user {username} unfollowed {profileUser.Username}");

        TempData.QueueFlashMessage($"You are no longer following \"{profileUser.Username}\"");

        return RedirectToAction("Index", new { username });
    }

    [HttpPost("add_message")]
    public async Task<IActionResult> AddMessage(string text)
    {
        bool is_loggedin = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
        if (!is_loggedin)
        {
            _logger.LogWarning($"AddMessage returns Unauthorized because user is not logged in");
            return Unauthorized();
        }

        var authorId = (int)HttpContext.Session.GetInt32("user_id");
        await _messageRepository.AddMessageAsync(text, authorId);
        _logger.LogInformation($"AddMessage added message for user with id {authorId} and text {text}");

        TempData.QueueFlashMessage("Your message was recorded");

        return RedirectToAction("Index");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        _logger.LogWarning($"Redirecting to error view");
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task<TimelineViewModel?> GetUserTimelineModelAsync(string username, bool is_loggedin)
    {
        User? profileUser = await _userRepository.GetUserAsync(username);
        if (profileUser == null)
        {
            _logger.LogWarning($"GetUserTimelineModel returns null because user {username} does not exist");
            return null;
        }

        var messages = await _messageRepository.GetUserSpecificMessagesAsync(profileUser, 30);

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
            model.Profile.IsFollowing = await _followerRepository.IsFollowingAsync(currentUserId, profileUser.UserId);
            model.CurrentUsername = (await _userRepository.GetUserAsync(currentUserId)).Username;
        }

        //Is this really needed? Nothing has changed since the last call in line 142, as far as i can see.
        model.Messages = await _messageRepository.GetUserSpecificMessagesAsync(profileUser, 30);

        _logger.LogInformation($"GetUserTimelineModel returns the Messages relevant for user {username}");
        return model;
    }

    private async Task<TimelineViewModel> GetCurrentUserTimelineModelAsync(int currentUserId)
    {
        var currentUsername = (await _userRepository.GetUserAsync(currentUserId))?.Username;

        var messages = await _messageRepository.GetCurrentUserSpecificMessagesAsync(currentUserId, 30);

        _logger.LogInformation($"GetCurrentUserTimelineModel returns the Timeline relevant for the current user");

        return new TimelineViewModel { CurrentUsername = currentUsername, Messages = messages };
    }
}
