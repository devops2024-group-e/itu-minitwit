using Microsoft.EntityFrameworkCore;
using Minitwit.Infrastructure.Repositories;
using Minitwit.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Minitwit.Infrastructure.Models;

public class MessageRepositoryTests
{
    private readonly IMessageRepository _messageRepo;

    private readonly User _user1;
    public MessageRepositoryTests()
    {
        var builder = new DbContextOptionsBuilder<MinitwitContext>();
        builder.UseInMemoryDatabase("MinitwitUserTestDB");
        var context = new MinitwitContext(builder.Options);
        context.Database.EnsureCreated();

        context.Messages.Add(new Message{MessageId = 1, AuthorId = 1, Text = "hello guys", PubDate = 1, Flagged = 0});
        context.Messages.Add(new Message{MessageId = 2, AuthorId = 2, Text = "Nice weather today.", PubDate = 2, Flagged = 0});
        context.Messages.Add(new Message{MessageId = 3, AuthorId = 3, Text = "I am flagged", PubDate = 3, Flagged = 1});

        _user1 = new User{UserId = 1, Email = "1@g.dk", Username = "user1", PwHash = "abcd"};
        context.Users.Add(_user1);

        _messageRepo = new MessageRepository(context);

        context.SaveChanges();
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
}