using Minitwit.Infrastructure.Models;

namespace MinitwitSimulatorAPI.Models;

public class MessageAuthor
{
    public Message Message { get; set; }
    public User Author { get; set; }
}
