using itu_new_minitwit.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace itu_new_minitwit.Pages;

public class PublicModel : PageModel
{
    private readonly MinitwitContext _context;
    private readonly ILogger<PublicModel> _logger;

    public ICollection<MessageAuthor> Messages { get; set; } = Enumerable.Empty<MessageAuthor>().ToList();

    public PublicModel(ILogger<PublicModel> logger, MinitwitContext context)
    {
        _context = context;
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation("We are at the public site!");
        Messages = (from message in _context.Messages
                    join user in _context.Users on message.AuthorId equals user.UserId
                    where message.Flagged == 0
                    orderby message.PubDate descending
                    select new MessageAuthor { Message = message, Author = user }).Take(30).ToList();

        _logger.LogInformation(Messages.Count.ToString());
    }
}
