using Microsoft.EntityFrameworkCore;
using Minitwit.Infrastructure.Repositories;
using Minitwit.Infrastructure;
using Minitwit.Infrastructure.Models;
namespace Minitwit.Infrastructure.Tests.Repositories;
public class MessageRepositoryTests : IDisposable
{
    private readonly IMessageRepository _messageRepo;
    private readonly MinitwitContext _context;

    private readonly User _user1;

    public MessageRepositoryTests()
    {
        var builder = new DbContextOptionsBuilder<MinitwitContext>();
        builder.UseInMemoryDatabase("MinitwitUserTestDB");
        _context = new MinitwitContext(builder.Options);
        _context.Database.EnsureCreated();

        _user1 = new User{Email = "1@g.dk", Username = "user1", PwHash = "abcd"};
        User _user2 = new User{Email = "2@g.dk", Username = "user2", PwHash = "efgh"};
        User _user3 = new User{Email = "3@g.dk", Username = "user3", PwHash = "ijkl"};
        _context.Users.Add(_user1);
        _context.Users.Add(_user2);
        _context.Users.Add(_user3);

        _context.SaveChanges();

        var us1 = _context.Users.Single(x => x.Username == "user1");
        var us2 = _context.Users.Single(x => x.Username == "user2");
        var us3 = _context.Users.Single(x => x.Username == "user3");

        _context.Messages.Add(new Message{AuthorId = us1.UserId, Text = "hello guys", PubDate = 1, Flagged = 0});
        _context.Messages.Add(new Message{AuthorId = us2.UserId, Text = "Nice weather today.", PubDate = 2, Flagged = 0});
        _context.Messages.Add(new Message{AuthorId = us3.UserId, Text = "I am nice", PubDate = 3, Flagged = 0});

        _messageRepo = new MessageRepository(_context);

        _context.SaveChanges();
    }

    [Fact]
    public void AddMessageReturnsTrue()
    {
        var response = _messageRepo.AddMessage("Hello", 2);

        Assert.True(response);
    }

    [Fact]
    public void GetMessagesReturnsRightMessages()
    {
        var response = _messageRepo.GetMessages();

        Assert.Equal(3, response.Count);
    }

    [Fact]
    public void GetUserSpecificMessagesReturnsRightMessages()
    {
        var response = _messageRepo.GetUserSpecificMessages(_user1);

        Assert.Equal(1, response.Count);
        Assert.Equal("hello guys", response[0].Message.Text);
    }

    [Fact]
    public void GetCurrentUserSpecificMessagesReturnsRightMessages()
    {
        var response = _messageRepo.GetCurrentUserSpecificMessages(_context.Users.Single(x => x.Username == "user1").UserId);

        Assert.Equal(1, response.Count);
        Assert.Equal("hello guys", response[0].Message.Text);
    }

    public void Dispose()
    {
        if (_context is null) return;

        var testMessages = _context.Users.Join(_context.Messages, x => x.UserId, x => x.AuthorId, (user, message) => new { user.Username, Message = message }).Where(x => x.Username.StartsWith("user"))
                                            .Select(x => x.Message);
        _context.Messages.RemoveRange(testMessages);
        _context.Users.RemoveRange(_context.Users.Where(x => x.Username.StartsWith("user")));
        _context.SaveChanges();
    }
}