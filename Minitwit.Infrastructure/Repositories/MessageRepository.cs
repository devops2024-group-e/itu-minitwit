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
        Task<bool> addingMessage = this.AddMessageAsync(text, authorId);
        addingMessage.Wait();

        return addingMessage.Result;
    }

    public List<MessageAuthor> GetMessages(int no)
    {
        Task<List<MessageAuthor>> gettingMessage = this.GetMessagesAsync(no);
        gettingMessage.Wait();

        return gettingMessage.Result;
    }

    public List<MessageAuthor> GetUserSpecificMessages(User profileUser, int no)
    {
        Task<List<MessageAuthor>> gettingMessage = this.GetUserSpecificMessagesAsync(profileUser, no);
        gettingMessage.Wait();

        return gettingMessage.Result;
    }

    public List<MessageAuthor> GetCurrentUserSpecificMessages(int currentUserId, int no)
    {
        Task<List<MessageAuthor>> gettingMessage = this.GetCurrentUserSpecificMessagesAsync(currentUserId, no);
        gettingMessage.Wait();

        return gettingMessage.Result;
    }

    public async Task<bool> AddMessageAsync(string text, int authorId)
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
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public Task<List<MessageAuthor>> GetMessagesAsync(int no)
        => Task.Run(() =>
    {

        return (from message in _context.Messages
                join user in _context.Users on message.AuthorId equals user.UserId
                where message.Flagged == 0
                orderby message.PubDate descending
                select new MessageAuthor { Message = message, Author = user }).Take(no).ToList();

    });

    public Task<List<MessageAuthor>> GetUserSpecificMessagesAsync(User profileUser, int no)
        => Task.Run(() =>
    {

        return (from message in _context.Messages
                join user in _context.Users on message.AuthorId equals user.UserId
                where user.UserId == profileUser.UserId
                orderby message.PubDate descending
                select new MessageAuthor { Message = message, Author = user }).Take(no).ToList();

    });

    public Task<List<MessageAuthor>> GetCurrentUserSpecificMessagesAsync(int currentUserId, int no)
        => Task.Run(() =>
    {
        return (from user in _context.Users
                join msg in _context.Messages on user.UserId equals msg.AuthorId
                where msg.Flagged == 0 && (user.UserId == currentUserId || (from f in _context.Followers
                                                                            where f.WhoId == currentUserId
                                                                            select f.WhomId).Any(x => x == user.UserId))
                orderby msg.PubDate descending
                select new MessageAuthor { Author = user, Message = msg }).Take(no).ToList();
    });
}
