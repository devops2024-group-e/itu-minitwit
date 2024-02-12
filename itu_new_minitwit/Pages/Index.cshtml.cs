using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using itu_new_minitwit.Model;

namespace itu_new_minitwit.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public string Timeline{ get; set; }

    public string Title{ get; set; }
    

    public List<Message> Messages { get; set; } = new List<Message>{
        new Message{Username = "Amalie", Text = "Helloooooo Hanne", PublishedDate = DateTime.Now },
        new Message{Username = "Malin", Text = "Who is Hanne?", PublishedDate = DateTime.Now.AddMinutes(1)}
    };

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet(string title)
    {   
        if(title == null)
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
        Messages.Add(new Message{Username = "My", Text = Request.Form["message"], PublishedDate = DateTime.Now});
    }
}
