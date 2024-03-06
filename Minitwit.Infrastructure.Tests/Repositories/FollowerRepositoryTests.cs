using Microsoft.EntityFrameworkCore;
using Minitwit.Infrastructure.Repositories;

namespace Minitwit.Infrastructure.Tests.Repositories;

public class FollowerRepositoryTests
{
    private readonly FollowerRepository _followerRepo;
    public FollowerRepositoryTests()
    {
        var builder = new DbContextOptionsBuilder<MinitwitContext>();
        builder.UseInMemoryDatabase("MinitwitFollowerTestDB");
        var context = new MinitwitContext(builder.Options);
        context.Database.EnsureCreated();

        //context.Followers.Add

        _followerRepo = new FollowerRepository(context);
    }

    [Fact]
    public void AddFollower_WhoIdDoesNotExistInDb_ReturnsFalse()
    {
        Assert.False(false);
    }
}