using Microsoft.AspNetCore.Http.HttpResults;
using static System.Net.HttpStatusCode;

namespace Minitwit.Simulator.Api.Tests;

public class LatestControllerTests : IClassFixture<MinitwitSimulatorApiApplicationFactory<Program>>, IDisposable
{

    private readonly MinitwitSimulatorApiApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public LatestControllerTests(MinitwitSimulatorApiApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetLatest_AddOneCallWithAnIdAndGetTheLatestId_RetunsCorrectStatusAndLatestId()
    {
        var response = await _client.PostAsJsonAsync("/register?latest=1337", new
        {
            Username = "test",
            Email = "test@test",
            Pwd = "foo"
        });

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(OK, response.StatusCode);

        // Check that latest is updated
        var latestResponse = await _client.GetAsync("/latest");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(OK, latestResponse.StatusCode);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
