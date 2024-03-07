using static System.Net.HttpStatusCode;
using MinitwitSimulatorAPI;
using MinitwitSimulatorAPI.Models;
using System.Net.Http.Headers;

namespace Minitwit.Simulator.Api.Tests.Controllers;

[Collection("SimulatorTest_Sequential")]
public class TimelineControllerTests : IClassFixture<MinitwitSimulatorApiApplicationFactory<Program>>, IDisposable
{
    private const string USERNAME_PREFIX = "TimelineApi";

    private readonly MinitwitSimulatorApiApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TimelineControllerTests(MinitwitSimulatorApiApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "c2ltdWxhdG9yOnN1cGVyX3NhZmUh");

        _client.RegisterUserAsync($"{USERNAME_PREFIX}a", "a@a.a", "a", 1001).Wait();
    }

    [Fact]
    public async Task CreateMessage_MessageWithExistingUser_ReturnsNoContentStatusCode()
    {
        var response = await _client.CreateMessageAsync("Blub!", $"{USERNAME_PREFIX}a", 1002);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(NoContent, response.StatusCode);

        var latestResponse = await _client.GetFromJsonAsync<LatestDTO>("/latest");
        if (latestResponse is null)
            Assert.Fail("Could not get latest result");

        Assert.Equal(1002, latestResponse.Latest);
    }

    [Fact]
    public async Task GetMessages_GetThe20LastMessages_ReturnOkStatusCodeAndListOfMessages()
    {
        await _client.CreateMessageAsync("Blub!", $"{USERNAME_PREFIX}a", 1003);
        var response = await _client.GetLatestMessagesAsync(20, latest: 1004);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(OK, response.StatusCode);

        var messages = await response.Content.ReadFromJsonAsync<List<MessageDTO>>();

        if (messages is null)
            Assert.Fail("'messages' is null");

        bool hasMessage = messages.Any(x => x.Content == "Blub!" && x.User == $"{USERNAME_PREFIX}a");
        Assert.True(hasMessage);

        var latestResponse = await _client.GetFromJsonAsync<LatestDTO>("/latest");
        if (latestResponse is null)
            Assert.Fail("could not the latest command");

        Assert.Equal(1004, latestResponse.Latest);
    }

    [Fact]
    public async Task GetUserMessages_GetThe20LastMessages_ReturnOkStatusCodeAndListOfMessages()
    {
        await _client.CreateMessageAsync("Blub!", $"{USERNAME_PREFIX}a", 1005);
        var response = await _client.GetLatestUserMessagesAsync($"{USERNAME_PREFIX}a", 20, latest: 1006);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(OK, response.StatusCode);

        var messages = await response.Content.ReadFromJsonAsync<List<MessageDTO>>();

        if (messages is null)
            Assert.Fail("'messages' is null");

        bool hasMessage = messages.Any(x => x.Content == "Blub!" && x.User == $"{USERNAME_PREFIX}a");
        Assert.True(hasMessage);

        var latestResponse = await _client.GetFromJsonAsync<LatestDTO>("/latest");
        if (latestResponse is null)
            Assert.Fail("could not the latest command");

        Assert.Equal(1006, latestResponse.Latest);
    }

    [Fact]
    public async Task FollowUser_FollowsUserGetMessages_ReturnPositiveCodesAndFollowsUser()
    {

        _client.RegisterUserAsync($"{USERNAME_PREFIX}b", "b@a.a", "b", 1007).Wait();
        _client.RegisterUserAsync($"{USERNAME_PREFIX}c", "c@a.a", "c", 1008).Wait();

        // A follows b and c
        var followResponse = await _client.FollowUserAsync($"{USERNAME_PREFIX}a", $"{USERNAME_PREFIX}b", 1009);

        Assert.True(followResponse.IsSuccessStatusCode);
        Assert.Equal(NoContent, followResponse.StatusCode);

        followResponse = await _client.FollowUserAsync($"{USERNAME_PREFIX}a", $"{USERNAME_PREFIX}c", 1010);

        Assert.True(followResponse.IsSuccessStatusCode);
        Assert.Equal(NoContent, followResponse.StatusCode);

        // Get a list of a's followers
        var followersResponse = await _client.GetAsync($"/fllws/{USERNAME_PREFIX}a?no=20&latest={1011}");

        Assert.True(followersResponse.IsSuccessStatusCode);
        Assert.Equal(OK, followersResponse.StatusCode);

        var followers = await followersResponse.Content.ReadFromJsonAsync<FollowerDTO>();
        if (followers is null)
            Assert.Fail("Could not get followers");

        Assert.Contains($"{USERNAME_PREFIX}b", followers.Follows);
        Assert.Contains($"{USERNAME_PREFIX}c", followers.Follows);

        var latestResponse = await _client.GetFromJsonAsync<LatestDTO>("/latest");
        if (latestResponse is null)
            Assert.Fail("Could not get latest response");

        Assert.Equal(1011, latestResponse.Latest);
    }

    [Fact]
    public async Task UnfollowUser_UnfollowUsersRemoveThemFromTheirList_ReturnsCorrectStatusCode()
    {
        await _client.RegisterUserAsync($"{USERNAME_PREFIX}b", "b@a.a", "b", 1012);
        await _client.FollowUserAsync($"{USERNAME_PREFIX}a", $"{USERNAME_PREFIX}b", 1013);

        var unfollowResponse = await _client.UnfollowUserAsync($"{USERNAME_PREFIX}a", $"{USERNAME_PREFIX}b", 1014);

        Assert.True(unfollowResponse.IsSuccessStatusCode);
        Assert.Equal(NoContent, unfollowResponse.StatusCode);

        // Get a list of a's followers
        var followersResponse = await _client.GetAsync($"/fllws/{USERNAME_PREFIX}a?no=20&latest={1015}");

        Assert.True(followersResponse.IsSuccessStatusCode);
        Assert.Equal(OK, followersResponse.StatusCode);

        var followers = await followersResponse.Content.ReadFromJsonAsync<FollowerDTO>();
        if (followers is null)
            Assert.Fail("Could not get followers");

        Assert.DoesNotContain($"{USERNAME_PREFIX}b", followers.Follows);

        var latestResponse = await _client.GetFromJsonAsync<LatestDTO>("/latest");
        if (latestResponse is null)
            Assert.Fail("Could not get latest response");

        Assert.Equal(1015, latestResponse.Latest);
    }

    public void Dispose()
    {
        using (var cleanUpScope = _factory.Services.CreateScope())
        {
            var context = cleanUpScope.ServiceProvider.GetService<MinitwitContext>();

            if (context is null)
                return;

            var commandIds = new List<int> { 1001, 1002, 1003, 1005, 1006, 1007, 1008, 1009, 1010, 1011, 1012, 1013, 1014, 1015 };
            var latestCommand = context.Latests.Where(x => commandIds.Contains(x.CommandId));

            if (latestCommand is not null)
                context.Latests.RemoveRange(latestCommand);

            var testMessages = context.Users.Join(context.Messages, x => x.UserId, x => x.AuthorId, (user, message) => new { user.Username, Message = message })
                                            .Where(x => x.Username.StartsWith(USERNAME_PREFIX))
                                            .Select(x => x.Message);

            context.Messages.RemoveRange(testMessages);

            var testFollowers = context.Users.Join(context.Followers, x => x.UserId, x => x.WhomId, (user, follower) => new { user.Username, Follower = follower })
                                            .Where(x => x.Username.StartsWith(USERNAME_PREFIX))
                                            .Select(x => x.Follower);

            context.Followers.RemoveRange(testFollowers);

            context.Users.RemoveRange(context.Users.Where(x => x.Username.StartsWith(USERNAME_PREFIX)));
            context.SaveChanges();
        }
    }
}
