using Minitwit.Infrastructure;
using Minitwit.Tests.Utils;

namespace Minitwit.Tests.IntegrationTests;

/// <summary>
/// Implements tests regarding the timeline. This mostly involves the TimelineController
/// </summary>
[Collection(nameof(SequentialControllerTestDefinition))]
public class TimelineTests : IClassFixture<MinitwitApplicationFactory<Program>>, IDisposable
{
    private const string USERNAME_PREFIX = "TimelineUi";

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

    [Fact]
    public async Task TestTimeline_FollowAndUnfollowInteractionAndAddingMessages_ShowsCorrectTimelines()
    {
        string messageFoo = "the message by foo";
        string messageBar = "the message by bar";

        // See that public timeline works
        await _client.LoginUserAsync($"{USERNAME_PREFIX}Foo", "default");
        await _client.AddMessageAsync(messageFoo);
        await _client.LogoutUserAsync();

        await _client.LoginUserAsync($"{USERNAME_PREFIX}Bar", "default");
        await _client.AddMessageAsync(messageBar);

        var publicContent = await _client.GetPageAsync("public");
        Assert.Contains(messageFoo, publicContent);
        Assert.Contains(messageBar, publicContent);

        // bar's timeline should jsut show bar's message
        var barPublicTimeline = await _client.GetPageAsync();
        Assert.Contains(messageBar, barPublicTimeline);
        Assert.DoesNotContain(messageFoo, barPublicTimeline);

        // Let bar follow foo
        var afterFollowContent = await _client.GetPageAsync($"{USERNAME_PREFIX}Foo/follow");
        Assert.Contains("You are now following &quot;TimelineUiFoo&quot;", afterFollowContent);

        // Foo's message should be visible in bars homepage
        barPublicTimeline = await _client.GetPageAsync();
        Assert.Contains(messageFoo, barPublicTimeline);
        Assert.Contains(messageBar, barPublicTimeline);

        // On user timeline pages we should only see the user specific messages
        var fooTimeline = await _client.GetPageAsync($"{USERNAME_PREFIX}Foo");
        Assert.DoesNotContain(messageBar, fooTimeline);
        Assert.Contains(messageFoo, fooTimeline);

        var barTimeline = await _client.GetPageAsync($"{USERNAME_PREFIX}Bar");
        Assert.Contains(messageBar, barTimeline);
        Assert.DoesNotContain(messageFoo, barTimeline);

        // Unfollow and see that foo is no longer present in bar's timeline feed
        var unfollowResponse = await _client.GetPageAsync($"{USERNAME_PREFIX}Foo/unfollow");
        Assert.Contains("You are no longer following &quot;TimelineUiFoo&quot;", unfollowResponse);

        var timelineContent = await _client.GetPageAsync();
        Assert.DoesNotContain(messageFoo, timelineContent);
        Assert.Contains(messageBar, timelineContent);
    }

    public void Dispose()
    {
        using (var cleanUpScope = _factory.Services.CreateScope())
        {
            var context = cleanUpScope.ServiceProvider.GetService<MinitwitContext>();

            if (context is null)
                return;

            context.Users.RemoveRange(context.Users.Where(x => x.Username.StartsWith(USERNAME_PREFIX)));

            // Remove test messages
            var testMessages = context.Users.Join(context.Messages, x => x.UserId, x => x.AuthorId, (user, message) => new { user.Username, Message = message })
                                            .Where(x => x.Username.StartsWith(USERNAME_PREFIX))
                                            .Select(x => x.Message);

            context.Messages.RemoveRange(testMessages);

            // Remove test followers
            var testFollowers = context.Users.Join(context.Followers, x => x.UserId, x => x.WhomId, (user, follower) => new { user.Username, Follower = follower })
                                            .Where(x => x.Username.StartsWith(USERNAME_PREFIX))
                                            .Select(x => x.Follower);

            context.Followers.RemoveRange(testFollowers);
            context.SaveChanges();
        }
    }
}
