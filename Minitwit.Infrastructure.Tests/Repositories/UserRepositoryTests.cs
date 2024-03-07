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

        _context.Users.Add(new Models.User{UserId = 1, Email = "1@g.dk", Username = "user1", PwHash = "abcd"});
        _context.Users.Add(new Models.User{UserId = 2, Email = "2@g.dk", Username = "user2", PwHash = "efgh"});
        _context.Users.Add(new Models.User{UserId = 3, Email = "3@g.dk", Username = "user3", PwHash = "ijkl"});

        _userRepo = new UserRepository(_context);

        _context.SaveChanges();
    }

    [Fact]
    public void GetUserFromUsernameReturnsRightUser()
    {
        var response = _userRepo.GetUser("user1");

        Assert.Equal(1, response.UserId);
        Assert.Equal("1@g.dk", response.Email);
        Assert.Equal("user1", response.Username);
        Assert.Equal("abcd", response.PwHash);
    }

    [Fact]
    public void GetUserFromIdReturnsRightUser()
    {
        var response = _userRepo.GetUser(2);

        Assert.Equal(2, response.UserId);
        Assert.Equal("2@g.dk", response.Email);
        Assert.Equal("user2", response.Username);
        Assert.Equal("efgh", response.PwHash);
    }

    [Fact]
    public void DoesUsereExistReturnsTrue()
    {
        var response = _userRepo.DoesUserExist("user3");

        Assert.True(response);
    }

    [Fact]
    public void DoesUsereExistReturnsFalse()
    {
        var response = _userRepo.DoesUserExist("user4");

        Assert.False(response);
    }

    [Fact]
    public void AddUserReturnsTrue()
    {
        var response = _userRepo.AddUser("newUser", "adsj@ad.dk", "szdhi");

        Assert.True(response);
    }

    public void Dispose()
    {
        if (_context is null) return;

        _context.Database.EnsureDeleted();
    }
}