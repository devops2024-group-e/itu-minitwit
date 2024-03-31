using Microsoft.EntityFrameworkCore;
using Minitwit.Infrastructure.Repositories;
using Minitwit.Infrastructure.Models;

namespace Minitwit.Infrastructure.Tests.Repositories;

public class LatestRepositoryTests : IDisposable
{
    private readonly LatestRepository _latestRepository;
    private readonly MinitwitContext _context;

    public LatestRepositoryTests()
    {
        var builder = new DbContextOptionsBuilder<MinitwitContext>();
        builder.UseInMemoryDatabase("MinitwitLatestTestDB");
        _context = new MinitwitContext(builder.Options);
        _context.Database.EnsureCreated();

        _context.Latests.Add(new Latest { Id = 1, CommandId = 11 });
        _context.Latests.Add(new Latest { Id = 2, CommandId = 22 });
        _context.Latests.Add(new Latest { Id = 3, CommandId = 33 });
        _context.Latests.Add(new Latest { Id = 4, CommandId = 44 });
        _context.SaveChanges();

        _latestRepository = new LatestRepository(_context);
    }

    [Fact]
    public void AddLatest_SuccesfullyAddsLatest_ReturnsTrue()
    {
        var result = _latestRepository.AddLatest(55);

        Assert.True(result);
    }

    [Fact]
    public void GetLatest_SuccesfullyGetsLatest_ReturnsLatestsCommandId()
    {
        var result = _latestRepository.GetLatest();

        Assert.Equal(44, result);
    }

    [Fact]
    public void AddLatest_GetLatest_SuccesfullyGetsNewlyAddedLatest_ReturnsLatestsCommandId()
    {
        // Arrange
        _latestRepository.AddLatest(55);

        // Act
        var result = _latestRepository.GetLatest();

        // Assert
        Assert.Equal(55, result);
    }

    public void Dispose()
    {
        if (_context is null) return;

        _context.Database.EnsureDeleted();
    }
}
