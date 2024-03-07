using Microsoft.EntityFrameworkCore;
using Minitwit.Infrastructure.Repositories;
using Minitwit.Infrastructure;
using Minitwit.Infrastructure.Models;

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

        _context.Messages.Add(new Message{MessageId = 1, AuthorId = 1, Text = "hello guys", PubDate = 1, Flagged = 0});
        _context.Messages.Add(new Message{MessageId = 2, AuthorId = 2, Text = "Nice weather today.", PubDate = 2, Flagged = 0});
        _context.Messages.Add(new Message{MessageId = 3, AuthorId = 3, Text = "I am nice", PubDate = 3, Flagged = 0});

        _user1 = new User{UserId = 1, Email = "1@g.dk", Username = "user1", PwHash = "abcd"};
        User _user2 = new User{UserId = 2, Email = "2@g.dk", Username = "user2", PwHash = "efgh"};
        User _user3 = new User{UserId = 3, Email = "3@g.dk", Username = "user3", PwHash = "ijkl"};
        _context.Users.Add(_user1);
        _context.Users.Add(_user2);
        _context.Users.Add(_user3);

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
        Assert.Equal(1, response[0].Message.MessageId);
        Assert.Equal(2, response[1].Message.MessageId);
        Assert.Equal(3, response[2].Message.MessageId);
    }

    [Fact]
    public void GetUserSpecificMessagesReturnsRightMessages()
    {
        var response = _messageRepo.GetUserSpecificMessages(_user1);

        Assert.Equal(1, response.Count);
        Assert.Equal(1, response[0].Message.MessageId);
        Assert.Equal("hello guys", response[0].Message.Text);
    }

    [Fact]
    public void GetCurrentUserSpecificMessagesReturnsRightMessages()
    {
        var response = _messageRepo.GetCurrentUserSpecificMessages(1);

        Assert.Equal(1, response.Count);
        Assert.Equal(1, response[0].Message.MessageId);
        Assert.Equal("hello guys", response[0].Message.Text);
    }

    public void Dispose()
    {
        if (_context is null) return;

        _context.Database.EnsureDeleted();
    }
}