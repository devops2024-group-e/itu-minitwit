namespace Minitwit.Infrastructure.Models;

public class MessageAuthor
{
    public Message Message { get; set; }
    public User Author { get; set; }
}
