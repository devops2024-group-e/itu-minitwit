using static System.Net.HttpStatusCode;
using MinitwitSimulatorAPI;
using MinitwitSimulatorAPI.Models;
using System.Net.Http.Headers;

namespace Minitwit.Simulator.Api.Tests.Controllers;

[Collection("SimulatorTest_Sequential")]
public class TimelineControllerTests : IClassFixture<MinitwitSimulatorApiApplicationFactory<Program>>, IDisposable
{
    private readonly MinitwitSimulatorApiApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TimelineControllerTests(MinitwitSimulatorApiApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "c2ltdWxhdG9yOnN1cGVyX3NhZmUh");

        _client.RegisterUserAsync("Timelinea", "a@a.a", "a", 1001).Wait();
    }

    [Fact]
    public async Task CreateMessage_MessageWithExistingUser_ReturnsNoContentStatusCode()
    {
        var response = await _client.CreateMessageAsync("Blub!", "Timelinea", 1002);

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
        await _client.CreateMessageAsync("Blub!", "Timelinea", 1003);
        var response = await _client.GetLatestMessagesAsync(20, latest: 1004);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(OK, response.StatusCode);

        var messages = await response.Content.ReadFromJsonAsync<List<MessageDTO>>();

        if (messages is null)
            Assert.Fail("'messages' is null");

        bool hasMessage = messages.Any(x => x.Content == "Blub!" && x.User == "Timelinea");
        Assert.True(hasMessage);

        var latestResponse = await _client.GetFromJsonAsync<LatestDTO>("/latest");
        if (latestResponse is null)
            Assert.Fail("could not the latest command");

        Assert.Equal(1004, latestResponse.Latest);
    }

    public void Dispose()
    {
        using (var cleanUpScope = _factory.Services.CreateScope())
        {
            var context = cleanUpScope.ServiceProvider.GetService<MinitwitContext>();

            if (context is null)
                return;

            var commandIds = new List<int> { 1001, 1002, 1003 };
            var latestCommand = context.Latests.Where(x => commandIds.Contains(x.CommandId));

            if (latestCommand is not null)
                context.Latests.RemoveRange(latestCommand);

            context.Users.RemoveRange(context.Users.Where(x => x.Username.StartsWith("Timeline")));
            context.SaveChanges();
        }
    }
}
