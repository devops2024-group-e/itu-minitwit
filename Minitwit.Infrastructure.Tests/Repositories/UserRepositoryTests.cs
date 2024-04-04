using Microsoft.EntityFrameworkCore;
using Minitwit.Infrastructure.Repositories;

namespace Minitwit.Infrastructure.Tests.Repositories;

[Collection(nameof(RepositorySequentialTestCollectionDefinition))]
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

        _context.Users.Add(new Models.User { Email = "1@g.dk", Username = "m_user1", PwHash = "abcd" });
        _context.Users.Add(new Models.User { Email = "2@g.dk", Username = "m_user2", PwHash = "efgh" });
        _context.Users.Add(new Models.User { Email = "3@g.dk", Username = "m_user3", PwHash = "ijkl" });

        _userRepo = new UserRepository(_context);

        _context.SaveChanges();
    }

    [Fact]
    public async Task GetUserFromUsernameReturnsRightUser()
    {
        var response = await _userRepo.GetUserAsync("m_user1");
        if (response is null)
            Assert.Fail("User is null");

        Assert.Equal("1@g.dk", response.Email);
        Assert.Equal("m_user1", response.Username);
        Assert.Equal("abcd", response.PwHash);
    }

    [Fact]
    public async Task GetUserFromIdReturnsRightUser()
    {
        var us = await _userRepo.GetUserAsync("m_user2");
        if (us is null)
            Assert.Fail("User m_user2 is null");

        var response = await _userRepo.GetUserAsync(us.UserId);
        if (response is null)
            Assert.Fail($"User with userid {us.UserId} return null");

        Assert.Equal(us.UserId, response.UserId);
        Assert.Equal("2@g.dk", response.Email);
        Assert.Equal("m_user2", response.Username);
        Assert.Equal("efgh", response.PwHash);
    }

    [Fact]
    public async Task DoesUsereExistReturnsTrue()
    {
        var response = await _userRepo.DoesUserExistAsync("m_user3");

        Assert.True(response);
    }

    [Fact]
    public async Task DoesUsereExistReturnsFalse()
    {
        var response = await _userRepo.DoesUserExistAsync("m_user4");

        Assert.False(response);
    }

    [Fact]
    public async Task AddUserReturnsTrue()
    {
        var response = await _userRepo.AddUserAsync("m_newUser", "adsj@ad.dk", "szdhi");

        Assert.True(response);
    }

    public void Dispose()
    {
        if (_context is null) return;
        _context.Users.RemoveRange(_context.Users.Where(x => x.Username.StartsWith("m_user")));
        _context.SaveChanges();
    }
}
