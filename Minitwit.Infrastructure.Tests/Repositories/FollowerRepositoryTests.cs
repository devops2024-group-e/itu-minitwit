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
    public void AddFollower_SuccesfullyAddsFollower_ReturnsTrue()
    {
        var result = _followerRepository.AddFollower(1, 2);

        Assert.True(result);
    }

    [Fact]
    public void RemoveFollower_RemovesFollower_ReturnsTrue()
    {
        // Arrange
        _followerRepository.AddFollower(1, 2);

        // Act
        var result = _followerRepository.RemoveFollower(1, 2);

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
        var result = _followerRepository.RemoveFollower(10020, 1111);

        Assert.True(result);
    }

    [Fact]
    public void RemoveFollower_SuccesfullyRemovesFollower_ReturnsTrue()
    {
        // Arrange
        _followerRepository.AddFollower(1, 2);

        // Act
        var removeResult = _followerRepository.RemoveFollower(1, 2);
        var isFollowingResult = _followerRepository.IsFollowing(1, 2);

        // Assert
        Assert.Multiple(
            () => Assert.True(removeResult),
            () => Assert.False(isFollowingResult)
        );

    }

    [Fact]
    public void IsFollowing_SuccesfullyChecksFollowing_ReturnsTrue()
    {
        // Arrange
        _followerRepository.AddFollower(1, 2);

        // Act
        var result = _followerRepository.IsFollowing(1, 2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsFollowing_ChecksFalseFollowing_ReturnsFalse()
    {
        var result = _followerRepository.IsFollowing(1, 3);

        Assert.False(result);
    }


    [Fact]
    public void GetCurrentUserFollows_SuccesfullyReturnsFollows_ReturnsUsername()
    {
        //Arrange
        _followerRepository.AddFollower(1, 2);

        //Act
        var result = _followerRepository.GetCurrentUserFollows(1, 1);

        //Assert
        Assert.Equal(new List<string> { "Bob" }, result);
    }

    [Fact]
    public void GetCurrentUserFollows_SuccesfullyReturnsMultipleFollows_ReturnsListOfUsernames()
    {
        //Arrange
        _followerRepository.AddFollower(1, 2);
        _followerRepository.AddFollower(1, 3);
        _followerRepository.AddFollower(1, 4);

        //Act
        var result = _followerRepository.GetCurrentUserFollows(1, 10);

        //Assert
        Assert.Equal(new List<string> { "Bob", "Charlie", "Dante" }, result);
    }

    public void Dispose()
    {
        if (_context is null) return;

        _context.Database.EnsureDeleted();
    }
}
