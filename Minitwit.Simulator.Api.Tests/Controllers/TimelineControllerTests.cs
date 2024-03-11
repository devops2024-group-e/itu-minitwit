using static System.Net.HttpStatusCode;
using MinitwitSimulatorAPI;
using MinitwitSimulatorAPI.Models;
using System.Net.Http.Headers;
using Xunit.Abstractions;

namespace Minitwit.Simulator.Api.Tests.Controllers;

[Collection(nameof(SequentialControllerTestCollectionDefinition))]
public class TimelineControllerTests
{
    private const string USERNAME_PREFIX = "TimelineApi";

    private readonly MinitwitSimulatorApiApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public TimelineControllerTests(ITestOutputHelper testOutputHelper, MinitwitSimulatorApiApplicationFactory<Program> factory)
    {
        _output = testOutputHelper;
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
        var userqResponse = await _client.RegisterUserAsync($"{USERNAME_PREFIX}q", "q@a.a", "q", 1007);
        var userwResponse = await _client.RegisterUserAsync($"{USERNAME_PREFIX}w", "w@a.a", "w", 1008);


        Assert.True(userqResponse.IsSuccessStatusCode);
        Assert.True(userwResponse.IsSuccessStatusCode);


        // A follows b and c
        var followResponse = await _client.FollowUserAsync($"{USERNAME_PREFIX}a", $"{USERNAME_PREFIX}q", 1009);

        _output.WriteLine(await followResponse.Content.ReadAsStringAsync());
        _output.WriteLine(followResponse.StatusCode.ToString());

        Assert.True(followResponse.IsSuccessStatusCode);
        Assert.Equal(NoContent, followResponse.StatusCode);

        followResponse = await _client.FollowUserAsync($"{USERNAME_PREFIX}a", $"{USERNAME_PREFIX}w", 1010);

        _output.WriteLine(await followResponse.Content.ReadAsStringAsync());
        _output.WriteLine(followResponse.StatusCode.ToString());

        Assert.True(followResponse.IsSuccessStatusCode);
        Assert.Equal(NoContent, followResponse.StatusCode);

        // Get a list of a's followers
        var followersResponse = await _client.GetAsync($"/fllws/{USERNAME_PREFIX}a?no=20&latest={1011}");

        Assert.True(followersResponse.IsSuccessStatusCode);
        Assert.Equal(OK, followersResponse.StatusCode);

        var followers = await followersResponse.Content.ReadFromJsonAsync<FollowerDTO>();
        if (followers is null)
            Assert.Fail("Could not get followers");

        Assert.Contains($"{USERNAME_PREFIX}q", followers.Follows);
        Assert.Contains($"{USERNAME_PREFIX}w", followers.Follows);

        var latestResponse = await _client.GetFromJsonAsync<LatestDTO>("/latest");
        if (latestResponse is null)
            Assert.Fail("Could not get latest response");

        Assert.Equal(1011, latestResponse.Latest);
    }

    [Fact]
    public async Task UnfollowUser_UnfollowUsersRemoveThemFromTheirList_ReturnsCorrectStatusCode()
    {
        await _client.RegisterUserAsync($"{USERNAME_PREFIX}a", "a@a.a", "a", 1001);
        await _client.RegisterUserAsync($"{USERNAME_PREFIX}z", "z@a.a", "z", 1012);

        await _client.FollowUserAsync($"{USERNAME_PREFIX}a", $"{USERNAME_PREFIX}z", 1013);

        var unfollowResponse = await _client.UnfollowUserAsync($"{USERNAME_PREFIX}a", $"{USERNAME_PREFIX}z", 1014);

        Assert.True(unfollowResponse.IsSuccessStatusCode);
        Assert.Equal(NoContent, unfollowResponse.StatusCode);

        // Get a list of a's followers
        var followersResponse = await _client.GetAsync($"/fllws/{USERNAME_PREFIX}a?no=20&latest={1015}");

        Assert.True(followersResponse.IsSuccessStatusCode);
        Assert.Equal(OK, followersResponse.StatusCode);

        var followers = await followersResponse.Content.ReadFromJsonAsync<FollowerDTO>();
        if (followers is null)
            Assert.Fail("Could not get followers");

        Assert.DoesNotContain($"{USERNAME_PREFIX}z", followers.Follows);

        var latestResponse = await _client.GetFromJsonAsync<LatestDTO>("/latest");
        if (latestResponse is null)
            Assert.Fail("Could not get latest response");

        Assert.Equal(1015, latestResponse.Latest);
    }
}
