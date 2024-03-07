using System.Net.Http.Headers;
using MinitwitSimulatorAPI;
using static System.Net.HttpStatusCode;

namespace Minitwit.Simulator.Api.Tests.Controllers;

public class RegisterControllerTests : IClassFixture<MinitwitSimulatorApiApplicationFactory<Program>>, IDisposable
{
    private readonly MinitwitSimulatorApiApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public RegisterControllerTests(MinitwitSimulatorApiApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("basic", "c2ltdWxhdG9yOnN1cGVyX3NhZmUh");
    }

    [Fact]
    public async Task Register_WithValidForm_ReturnsNoContentStatusCode()
    {
        var response = await _client.PostAsJsonAsync("/register?latest=1", new
        {
            Username = "Registera",
            Email = "a@a.a",
            Pwd = "a"
        });

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

            var latestCommand = context.Latests.Where(x => x.CommandId == 1);

            if (latestCommand is not null)
                context.Latests.RemoveRange(latestCommand);

            context.Users.RemoveRange(context.Users.Where(x => x.Username.StartsWith("Register")));
            context.SaveChanges();
        }
    }
}
