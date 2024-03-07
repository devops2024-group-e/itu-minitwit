using Microsoft.EntityFrameworkCore;
using Minitwit.Infrastructure.Repositories;

namespace Minitwit.Infrastructure.Tests.Repositories;

public class UserRepositoryTests : IDisposable
{

    private readonly IUserRepository _userRepo;
    private readonly MinitwitContext _context;
    public UserRepositoryTests()
    {
        var builder = new DbContextOptionsBuilder<MinitwitContext>();
        builder.UseInMemoryDatabase("MinitwitUserTestDB");
        _context = new MinitwitContext(builder.Options);
        _context.Database.EnsureCreated();

        _context.Users.Add(new Models.User{Email = "1@g.dk", Username = "m_user1", PwHash = "abcd"});
        _context.Users.Add(new Models.User{Email = "2@g.dk", Username = "m_user2", PwHash = "efgh"});
        _context.Users.Add(new Models.User{Email = "3@g.dk", Username = "m_user3", PwHash = "ijkl"});

        _userRepo = new UserRepository(_context);

        _context.SaveChanges();
    }

    [Fact]
    public void GetUserFromUsernameReturnsRightUser()
    {
        var response = _userRepo.GetUser("m_user1");

        Assert.Equal("1@g.dk", response.Email);
        Assert.Equal("m_user1", response.Username);
        Assert.Equal("abcd", response.PwHash);
    }

    [Fact]
    public void GetUserFromIdReturnsRightUser()
    {
        var us = _userRepo.GetUser("m_user2");
        var response = _userRepo.GetUser(us.UserId);

        Assert.Equal(us.UserId, response.UserId);
        Assert.Equal("2@g.dk", response.Email);
        Assert.Equal("m_user2", response.Username);
        Assert.Equal("efgh", response.PwHash);
    }

    [Fact]
    public void DoesUsereExistReturnsTrue()
    {
        var response = _userRepo.DoesUserExist("m_user3");

        Assert.True(response);
    }

    [Fact]
    public void DoesUsereExistReturnsFalse()
    {
        var response = _userRepo.DoesUserExist("m_user4");

        Assert.False(response);
    }

    [Fact]
    public void AddUserReturnsTrue()
    {
        var response = _userRepo.AddUser("m_newUser", "adsj@ad.dk", "szdhi");

        Assert.True(response);
    }

    public void Dispose()
    {
        if (_context is null) return;
        _context.Users.RemoveRange(_context.Users.Where(x => x.Username.StartsWith("m_user")));
        _context.SaveChanges();
    }
}