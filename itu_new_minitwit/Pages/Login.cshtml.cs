using itu_new_minitwit.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace itu_new_minitwit.Pages;

public class LoginModel : PageModel
{
    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string ErrorMessage { get; set; } = default!;

    private readonly List<UserCredential> _users = new()
    {
        new UserCredential { Username = "Testuser", Password = "sesam123" }
    };

    public void OnGet()
    {

    }

    public IActionResult OnPost()
    {
        var user = _users.SingleOrDefault(x => x.Username == Username);
        if (user == null)
        {
            ErrorMessage = "Invalid username or password";
            return Page();
        }


        if (user.Password == Password)
        {
            // TODO: Get user_id by username from database
            HttpContext.Session.SetInt32("user_id", 1);

            return RedirectToPage("/Index");
        }
        else
        {
            ErrorMessage = "Invalid username or password";
            return Page();
        }
    }
}
