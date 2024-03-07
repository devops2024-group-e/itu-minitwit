using System.Net.Http.Headers;
using MinitwitSimulatorAPI;
using static System.Net.HttpStatusCode;

namespace Minitwit.Simulator.Api.Tests.Controllers;

[Collection("SimulatorTest_Sequential")]
public class RegisterControllerTests : IClassFixture<MinitwitSimulatorApiApplicationFactory<Program>>, IDisposable
{
    private readonly MinitwitSimulatorApiApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public RegisterControllerTests(MinitwitSimulatorApiApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "c2ltdWxhdG9yOnN1cGVyX3NhZmUh");
    }

    [Theory]
    [InlineData("Registera", "a@a.a", "a", 1)]
    [InlineData("Registerb", "b@b.b", "b", 5)]
    [InlineData("Registerc", "c@c.c", "c", 6)]
    public async Task Register_WithValidForm_ReturnsNoContentStatusCode(string username, string email, string password, int latest)
    {
        var response = await _client.RegisterUserAsync(username, email, password, latest: latest);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(NoContent, response.StatusCode);
    }

    public void Dispose()
    {
        using (var cleanUpScope = _factory.Services.CreateScope())
        {
            var context = cleanUpScope.ServiceProvider.GetService<MinitwitContext>();

            if (context is null)
                return;

            var commandIds = new List<int> { 1, 5, 6 };
            var latestCommand = context.Latests.Where(x => commandIds.Contains(x.CommandId));

            if (latestCommand is not null)
                context.Latests.RemoveRange(latestCommand);

            context.Users.RemoveRange(context.Users.Where(x => x.Username.StartsWith("Register")));
            context.SaveChanges();
        }
    }
}
