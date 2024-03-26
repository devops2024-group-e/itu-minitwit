using Microsoft.EntityFrameworkCore;
using Minitwit.Infrastructure.Repositories;
using Minitwit.Infrastructure.Models;

namespace Minitwit.Infrastructure.Tests.Repositories;

public class FollowerRepositoryTests : IDisposable
{
    private readonly FollowerRepository _followerRepository;
    private readonly MinitwitContext _context;

    public FollowerRepositoryTests()
    {
        var builder = new DbContextOptionsBuilder<MinitwitContext>();
        builder.UseInMemoryDatabase("MinitwitFollowerTestDB");
        _context = new MinitwitContext(builder.Options);
        _context.Database.EnsureCreated();

        _context.Users.Add(new User { UserId = 1, Username = "Alice", Email = "example@mail.com", PwHash = "qwerty" });
        _context.Users.Add(new User { UserId = 2, Username = "Bob", Email = "example@mail.com", PwHash = "qwerty" });
        _context.Users.Add(new User { UserId = 3, Username = "Charlie", Email = "example@mail.com", PwHash = "qwerty" });
        _context.Users.Add(new User { UserId = 4, Username = "Dante", Email = "example@mail.com", PwHash = "qwerty" });
        _context.SaveChanges();

        _followerRepository = new FollowerRepository(_context);
    }

    [Fact]
    public async Task AddFollowerAsync_SuccesfullyAddsFollower_ReturnsTrue()
    {
        var result = await _followerRepository.AddFollowerAsync(1, 2);

        Assert.True(result);
    }

    [Fact]
    public async Task RemoveFollowerAsync_RemovesFollower_ReturnsTrue()
    {
        // Arrange
        await _followerRepository.AddFollowerAsync(1, 2);

        // Act
        var result = await _followerRepository.RemoveFollowerAsync(1, 2);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// When we remove a follower relation that does not exist then it should return true because it is not present anymore anyway
    /// </summary>
    [Fact]
    public void RemoveFollower_FollowerRelationDoesNotExistInDB_ReturnsTrue()
    {
        // Try to remove a follower relation that does not exist
        var result = await _followerRepository.RemoveFollowerAsync(10020, 1111);

        Assert.True(result);
    }

    [Fact]
    public async Task RemoveFollowerAsync_SuccesfullyRemovesFollower_ReturnsTrue()
    {
        // Arrange
        await _followerRepository.AddFollowerAsync(1, 2);

        // Act
        var removeResult = await _followerRepository.RemoveFollowerAsync(1, 2);
        var isFollowingResult = await _followerRepository.IsFollowingAsync(1, 2);

        // Assert
        Assert.Multiple(
            () => Assert.True(removeResult),
            () => Assert.False(isFollowingResult)
        );

    }

    [Fact]
    public async Task IsFollowing_SuccesfullyChecksFollowing_ReturnsTrue()
    {
        // Arrange
        await _followerRepository.AddFollowerAsync(1, 2);

        // Act
        var result = await _followerRepository.IsFollowingAsync(1, 2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsFollowing_ChecksFalseFollowing_ReturnsFalse()
    {
        var result = await _followerRepository.IsFollowingAsync(1, 3);

        Assert.False(result);
    }


    [Fact]
    public async Task GetCurrentUserFollows_SuccesfullyReturnsFollows_ReturnsUsername()
    {
        //Arrange
        await _followerRepository.AddFollowerAsync(1, 2);

        //Act
        var result = await _followerRepository.GetCurrentUserFollowsAsync(1, 1);

        //Assert
        Assert.Equal(new List<string> { "Bob" }, result);
    }

    [Fact]
    public async Task GetCurrentUserFollows_SuccesfullyReturnsMultipleFollows_ReturnsListOfUsernames()
    {
        //Arrange
        await _followerRepository.AddFollowerAsync(1, 2);
        await _followerRepository.AddFollowerAsync(1, 3);
        await _followerRepository.AddFollowerAsync(1, 4);

        //Act
        var result = await _followerRepository.GetCurrentUserFollowsAsync(1, 10);

        //Assert
        Assert.Equal(new List<string> { "Bob", "Charlie", "Dante" }, result);
    }

    public void Dispose()
    {
        if (_context is null) return;

        _context.Database.EnsureDeleted();
    }
}
