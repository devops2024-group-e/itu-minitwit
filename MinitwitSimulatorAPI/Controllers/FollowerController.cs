using Microsoft.AspNetCore.Mvc;
using Minitwit.Infrastructure.Models;
using Minitwit.Infrastructure.Repositories;
using MinitwitSimulatorAPI.Models;

namespace MinitwitSimulatorAPI.Controllers;


[Route("/fllws")]
public class FollowerController : Controller
{
    private readonly ILogger<FollowerController> _logger;
    private readonly ILatestRepository _latestRepository;
    private readonly IFollowerRepository _followerRepository;
    private readonly IUserRepository _userRepository;


    public FollowerController(ILogger<FollowerController> logger, ILatestRepository latestRepository, IFollowerRepository followerRepository, IUserRepository userRepository)
    {
        _logger = logger;
        _latestRepository = latestRepository;
        _followerRepository = followerRepository;
        _userRepository = userRepository;
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
    private async Task<User?> GetUserAsync(string username)
    {
        username = username.Replace('\n', '_').Replace('\r', '_');

        _logger.LogInformation($"GetUser return user with username {username}");
        return await _userRepository.GetUserAsync(username);
    }

    /// <summary>
    /// The user currently logged in, follows or unfollows the user given in the json-body.
    /// </summary>
    /// <param name="ownUsername">The username of the user to be followed.</param>
    /// <returns>Either Http code 404 (NotFound) or Http code 204 (Nocontent)</returns>
    ///
    [HttpPost("{username}")]
    public async Task<IActionResult> FollowUnfollowUser([FromQuery] int latest, string username, [FromBody] Dictionary<string, string> dicti)
    {
        username = username.Replace('\n', '_').Replace('\r', '_');

        await _latestRepository.AddLatestAsync(latest);
        _logger.LogDebug($"FollowUnfollowUser added latest: {latest}");
        var user = await this.GetUserAsync(username);
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

        if (dicti.Count > 1)
        {
            _logger.LogWarning("The dictionary contains more than one request");
            return BadRequest();
        }

        string otherUsername = dicti.First().Value;
        var otherUser = await this.GetUserAsync(otherUsername);
        if (otherUser is null)
        {
            _logger.LogWarning("FollowUnfollowUser returns NotFound because other username does not exist");
            return NotFound();
        }
        var otherUserId = otherUser.UserId;

        if (dicti.ContainsKey("follow"))
        {
            await _followerRepository.AddFollowerAsync(ownUserId, otherUserId);
            _logger.LogInformation($"FollowUnfollowUser user {ownUserId} follows {otherUserId}");
        }
        else if (dicti.ContainsKey("unfollow"))
        {
            await _followerRepository.RemoveFollowerAsync(ownUserId, otherUserId);
            _logger.LogInformation($"FollowUnfollowUser user {ownUserId} unfollows {otherUserId}");
        }

        _logger.LogWarning($"FollowUnfollowUser returns NoContent");
        return NoContent();
    }

    /// <summary>
    /// Gets the users followed by a given user.
    /// </summary>
    /// <param name="username">The username of the user, whose follows should be returned.</param>
    /// <returns>Either http code 404 (NotFound) or http code 200 (Ok)</returns>
    [HttpGet("{username}")]
    public async Task<ActionResult<FollowerDTO>> GetFollows([FromQuery] int latest, string username, [FromQuery] int no = 100)
    {
        username = username.Replace('\n', '_').Replace('\r', '_');

        await _latestRepository.AddLatestAsync(latest);
        _logger.LogDebug($"GetFollows added latest: {latest}");

        User? profileUser = await this.GetUserAsync(username);
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

        var follows = await _followerRepository.GetCurrentUserFollowsAsync(profileUser.UserId, no);

        _logger.LogInformation($"GetFollows returns followers for username {username}");
        return Ok(new FollowerDTO(follows));
    }
}
