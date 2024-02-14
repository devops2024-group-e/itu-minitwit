using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using itu_new_minitwit.Models;

namespace itu_new_minitwit.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    private MinitwitContext _context;

    public ICollection<MessageAuthor> Messages { get; set; } = Enumerable.Empty<MessageAuthor>().ToList();
    public bool Followed { get; set; }
    public string Username { get; set; }

    public IndexModel(ILogger<IndexModel> logger, MinitwitContext context)
    {
        _logger = logger;
        _context = context;
    }

    public long GetUserID(string username)
    {
        return _context.Users.Single(x => x.Username == username).UserId;
    }

    public void Follow(string usernameToFollow)
    {
        var ownUserID = HttpContext.Session.GetInt32("user_id");

        _context.Followers.Add(new Follower { WhoId = ownUserID, WhomId = GetUserID(usernameToFollow) });
        //flask: you are now following user usertofollow

    }

    public void Unfollow(string usernameToUnfollow)
    {
        var ownUserID = HttpContext.Session.GetInt32("user_id");
        var whomID = GetUserID(usernameToUnfollow);

        if (_context.Users.Select(x => x.UserId == ownUserID).ToList().Count == 1)
        {
            //My own user does not exist
        }
        if (_context.Users.Select(x => x.UserId == whomID).ToList().Count == 1)
        {
            //The user to unfollow does not exist
        }

        _context.Followers.Remove(_context.Followers.Single(x => x.WhoId == ownUserID && x.WhomId == whomID));
        //flask: you have now unfollowed user usertounfollow
    }

    public IActionResult OnGet()
    {
        string path = HttpContext.Request.Path;
        bool is_loggedin = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);

        if (!is_loggedin)
        {
            return RedirectToPage("/Public");
        }
        else
        {
            var userId = HttpContext.Session.GetInt32("user_id");
            Username = _context.Users.Single(x => x.UserId == userId).Username;

            Messages = (from message in _context.Messages
                        join user in _context.Users on message.AuthorId equals user.UserId
                        where message.Flagged == 0 && (from follower in _context.Followers
                                                       where follower.WhoId == userId
                                                       select follower.WhomId).Contains(message.AuthorId)
                        orderby message.PubDate descending
                        select new MessageAuthor { Author = user, Message = message }).Take(30).ToList();
        }

        return Page();
    }

    public void OnPost()
    {

    }
}
