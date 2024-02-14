using itu_new_minitwit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace itu_new_minitwit.Pages;

public class UserTimelineModel : PageModel
{
    private readonly MinitwitContext _context;

    public string Username { get; set; }
    public bool Followed { get; set; }
    public ICollection<MessageAuthor> Messages { get; set; }

    public UserTimelineModel(MinitwitContext dbcontext)
    {
        _context = dbcontext;
    }

    public IActionResult OnGet(string username)
    {
        bool is_loggedin = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
        User? profileUser = _context.Users.SingleOrDefault(x => x.Username == username);
        if (profileUser == null)
        {
            return NotFound();
        }

        Username = profileUser.Username;
        Followed = false;
        if (is_loggedin)
        {
            Followed = _context.Followers
                                .Any(x => x.WhoId == HttpContext.Session.GetInt32("user_id") && x.WhomId == profileUser.UserId);
        }

        Messages = (from message in _context.Messages
                    join user in _context.Users on message.AuthorId equals user.UserId
                    where user.UserId == profileUser.UserId
                    orderby message.PubDate descending
                    select new MessageAuthor { Message = message, Author = user }).Take(30).ToList();

        return Page();
    }

    public IActionResult OnGetFollow(string username)
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

        return RedirectToPage("/UserTimeline", new { username });
    }

    public IActionResult OnGetUnfollow(string username)
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

        return RedirectToPage("/UserTimeline", new { username });
    }
}
