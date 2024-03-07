using Minitwit.Tests.Utils;

namespace Minitwit.Tests.IntegrationTests;

/// <summary>
/// Implements tests regarding the timeline. This mostly involves the TimelineController
/// </summary>
public class TimelineTests : IClassFixture<MinitwitApplicationFactory<Program>>, IDisposable
{
    private const string USERNAME_PREFIX = "Timeline";

    private readonly MinitwitApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TimelineTests(MinitwitApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        _client.CreateTestUserAsync($"{USERNAME_PREFIX}Foo").Wait();
        _client.CreateTestUserAsync($"{USERNAME_PREFIX}Bar").Wait();
    }

    [Fact]
    public async Task AddMessage_AddMessagesGetsDisplayed_MessagesPresentInPublicTimeline()
    {
        // Arrange
        await _client.LoginUserAsync($"{USERNAME_PREFIX}Foo", "default");

        // Act
        await _client.AddMessageAsync("test message 1");
        await _client.AddMessageAsync("<test message 2>");

        var publicTimelineContent = await _client.GetPageAsync("public");

        Assert.Contains("test message 1", publicTimelineContent);
        Assert.Contains("&lt;test message 2&gt", publicTimelineContent);
    }

    public void Dispose()
    {
        using (var cleanUpScope = _factory.Services.CreateScope())
        {
            var context = cleanUpScope.ServiceProvider.GetService<MinitwitContext>();

            if (context is null)
                return;

            context.Users.RemoveRange(context.Users.Where(x => x.Username.StartsWith(USERNAME_PREFIX)));
            context.Messages.RemoveRange(context.Messages);
            context.Followers.RemoveRange(context.Followers); // TODO: Should maybe only be the users from this test
            context.SaveChanges();
        }
    }
}
