using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using itu_new_minitwit.Models;

namespace itu_new_minitwit.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public string Timeline { get; set; }

    public string Title { get; set; }

    private MinitwitContext _context;

    public List<Message> Messages { get; set; } = new List<Message>{};

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
        if(_context.Users.Select(x => x.UserId == ownUserID).ToList().Count == 1)
        {
            //My own user does not exist
        }
        if(_context.Users.Select(x => x.UserId == GetUserID(usernameToFollow)).ToList().Count == 1)
        {
            //The user to follow does not exist
        }
        
        _context.Followers.Add(new Follower{WhoId = ownUserID, WhomId = GetUserID(usernameToFollow)});
        //flask: you are now following user usertofollow
        
    }

    public void Unfollow(string usernameToUnfollow)
    {
        var ownUserID = HttpContext.Session.GetInt32("user_id");
        var whomID = GetUserID(usernameToUnfollow);

        if(_context.Users.Select(x => x.UserId == ownUserID).ToList().Count == 1)
        {
            //My own user does not exist
        }
        if(_context.Users.Select(x => x.UserId == whomID).ToList().Count == 1)
        {
            //The user to unfollow does not exist
        }

        _context.Followers.Remove(_context.Followers.Single(x => x.WhoId == ownUserID && x.WhomId == whomID));
        //flask: you have now unfollowed user usertounfollow
    }

    public void OnGet(string title)
    {
        // TODO: If we do not have a user in the session query public timeline
        // TODO: Add query for public timeline

        // TODO: Else get timeline from the users follow list
        if (title == null)
        {
            Title = "My";
        }
        else
        {
            Title = title;
        }
    }

    public void OnPost()
    {
        Messages.Add(new Message { Username = "My", Text = Request.Form["message"], PublishedDate = DateTime.Now });
    }
}
