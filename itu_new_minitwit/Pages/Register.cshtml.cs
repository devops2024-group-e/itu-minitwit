using itu_new_minitwit.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace itu_new_minitwit.Pages;

public class RegisterModel : PageModel
{
    private readonly ILogger<RegisterModel> _logger;
    private readonly List<UserCredential> _users = new()
    {
        new UserCredential { Username = "Testuser", Password = "sesam123" }
    };

    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = default!;

    public RegisterModel(ILogger<RegisterModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        bool is_authenticated = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
        if (is_authenticated)
        {
            _logger.LogDebug("User is already authenticated... redirecting to timeline");
            return RedirectToPage("/Index"); // TODO: Change to '/Timeline' ??
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        bool is_authenticated = HttpContext.Session.TryGetValue("user_id", out byte[]? bytes);
        if (is_authenticated)
        {
            _logger.LogDebug("User is already authenticated... redirecting to timeline");
            return RedirectToPage("/Index"); // TODO: Change to '/Timeline' ??
        }

        if (string.IsNullOrEmpty(Username))
            ErrorMessage = "Username is required";
        else if (string.IsNullOrEmpty(Email) || !Email.Contains("@"))
            ErrorMessage = "You have to enter a valid email address";
        else if (string.IsNullOrEmpty(Password))
            ErrorMessage = "Password is required";
        else if (Password != ConfirmPassword)
            ErrorMessage = "The two passwords do not match";
        else if (_users.Any(x => x.Username == Username))
            ErrorMessage = "The username is already taken";
        else
        {
            // TODO: Store the new user in the database with hashed password
            User user = new User // We should use another type to represent the model the database
            {
                Username = Username,
                Email = Email,
                Password = Password // TODO: Hash this
            };

            _logger.LogDebug("User registered: {Username}", Username);

            // TODO: Add popup message here!

            return RedirectToPage("/Login");
        }

        return Page();
    }
}
