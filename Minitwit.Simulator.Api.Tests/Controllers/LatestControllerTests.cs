using System.Net;
using System.Net.Http.Headers;
using MinitwitSimulatorAPI;
using static System.Net.HttpStatusCode;

namespace Minitwit.Simulator.Api.Tests.Controllers;

[Collection("SimulatorTest_Sequential")]
public class LatestControllerTests : IClassFixture<MinitwitSimulatorApiApplicationFactory<Program>>, IDisposable
{

    private readonly MinitwitSimulatorApiApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public LatestControllerTests(MinitwitSimulatorApiApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "c2ltdWxhdG9yOnN1cGVyX3NhZmUh");
    }

    [Fact]
    public async Task GetLatest_AddOneCallWithAnIdAndGetTheLatestId_RetunsCorrectStatusAndLatestId()
    {
        var response = await _client.PostAsJsonAsync("/register?latest=1337", new
        {
            Username = "Latesttest",
            Email = "test@test",
            Pwd = "foo"
        });

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(NoContent, response.StatusCode);

        // Check that latest is updated
        var latestResponse = await _client.GetAsync("/latest");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(OK, latestResponse.StatusCode);
    }

    public void Dispose()
    {
        using (var cleanUpScope = _factory.Services.CreateScope())
        {
            var context = cleanUpScope.ServiceProvider.GetService<MinitwitContext>();

            if (context is null)
                return;

            var latestCommand = context.Latests.Where(x => x.CommandId == 1337);

            if (latestCommand is not null)
                context.Latests.RemoveRange(latestCommand);

            context.Users.RemoveRange(context.Users.Where(x => x.Username.StartsWith("Latest")));
            context.SaveChanges();
        }
    }
}
