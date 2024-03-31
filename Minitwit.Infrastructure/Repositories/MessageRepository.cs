using Minitwit.Infrastructure.Models;
namespace Minitwit.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly MinitwitContext _context;

    public MessageRepository(MinitwitContext context)
    {
        _context = context;
    }

    public bool AddMessage(string text, int authorId)
    {
        _context.Messages.Add(new Message
        {
            AuthorId = authorId,
            Text = text,
            PubDate = (int)DateTime.Now.Ticks,
            Flagged = 0
        });
        try
        {
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }

    public List<MessageAuthor> GetMessages(int no)
    {
        var messages = (from message in _context.Messages
                        join user in _context.Users on message.AuthorId equals user.UserId
                        where message.Flagged == 0
                        orderby message.PubDate descending
                        select new MessageAuthor { Message = message, Author = user }).Take(no).ToList();

        return messages;
    }

    public List<MessageAuthor> GetUserSpecificMessages(User profileUser, int no)
    {
        var messages = (from message in _context.Messages
                        join user in _context.Users on message.AuthorId equals user.UserId
                        where user.UserId == profileUser.UserId
                        orderby message.PubDate descending
                        select new MessageAuthor { Message = message, Author = user }).Take(no).ToList();

        return messages;
    }

    public List<MessageAuthor> GetCurrentUserSpecificMessages(int currentUserId, int no)
    {
        var messages = (from user in _context.Users
                        join msg in _context.Messages on user.UserId equals msg.AuthorId
                        where msg.Flagged == 0 && (user.UserId == currentUserId || (from f in _context.Followers
                                                                                    where f.WhoId == currentUserId
                                                                                    select f.WhomId).Any(x => x == user.UserId))
                        orderby msg.PubDate descending
                        select new MessageAuthor { Author = user, Message = msg }).Take(no).ToList();

        return messages;
    }
}
