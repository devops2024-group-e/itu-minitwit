using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using itu_new_minitwit.Model;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace itu_new_minitwit.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public string Title { get; set; }

    private MinitwitContext _context;

    public List<Message> Messages { get; set; } = new List<Message>{
        new Message{Username = "Amalie", Text = "Helloooooo Hanne", PublishedDate = DateTime.Now },
        new Message{Username = "Malin", Text = "Who is Hanne?", PublishedDate = DateTime.Now.AddMinutes(1)}
    };

    public IndexModel(ILogger<IndexModel> logger, MinitwitContext context)
    {
        _logger = logger;
        _context = context;
    }

    public void follow(User userToFollow)
    {
        // TODO: Create a query to follow a specific user
        var ownUserID = HttpContext.Session.GetInt32("user_id");
        
    }

    public void unfollow(User userToUnfollow)
    {
        // TODO: Create a query to unfollow a specific user
    }

    public IActionResult OnGet()
    {   

        string path = HttpContext.Request.Path;

        if(path == "/Public")
        {
            Title = "Public";
        }
        else if(path != "/")
        {
            Title = path.Trim('/');
        }
        else
        {
            bool is_loggedin = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
            if(!is_loggedin)
            {
                return RedirectToPage("/Public");
            }
            else
            {
                Title = "My";
            }
        }

        // TODO: If we do not have a user in the session query public timeline
        


        
        // TODO: Add query for public timeline

        // TODO: Else get timeline from the users follow list
        
        /*
        if (title == null)
        {
            Title = "My";
        }
        else
        {
            Title = title;
        }*/
    }

    public void OnPost()
    {
        Messages.Add(new Message { Username = "My", Text = Request.Form["message"], PublishedDate = DateTime.Now });
    }
}
