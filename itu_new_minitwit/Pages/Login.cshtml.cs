using itu_new_minitwit.Model;
using itu_new_minitwit.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace itu_new_minitwit.Pages;

public class LoginModel : PageModel
{
    private readonly MinitwitContext _dbContext;

    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string ErrorMessage { get; set; } = default!;

    public LoginModel(MinitwitContext dbcontext)
    {
        _dbContext = dbcontext;
    }

    private readonly List<UserCredential> _users = new()
    {
        new UserCredential { Username = "Testuser", Password = "sesam123" }
    };

    public IActionResult OnGet()
    {
        bool is_authenticated = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
        if (is_authenticated)
            return RedirectToPage("/Index"); // TODO: Change to '/Timeline' ??

        return Page();
    }

    public IActionResult OnPost()
    {
        bool is_authenticated = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
        if (is_authenticated)
            return RedirectToPage("/Index"); // TODO: Change to '/Timeline' ??

        var user = _dbContext.Users.SingleOrDefault(x => x.Username == Username);
        if (user == null)
        {
            ErrorMessage = "Invalid username"; // NOTE: Potential security risk... not good to tell the username does not exist
            return Page();
        }

        var hashedPassword = PasswordHash.Hash(Password);
        if (user.PwHash == hashedPassword)
        {
            HttpContext.Session.SetInt32("user_id", (int)user.UserId); // TODO: This is a bad type conversion...

            return RedirectToPage("/Index");
        }
        else
        {
            ErrorMessage = "Invalid password"; // NOTE: Potential security risk... not good to tell the password is incorrect
            return Page();
        }
    }
}
