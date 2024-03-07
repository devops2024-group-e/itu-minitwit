using Microsoft.AspNetCore.Mvc.Testing;
using Minitwit.Tests.Utils;

namespace Minitwit.Tests.IntegrationTests;

public class MinitwitUserInteraction : IClassFixture<MinitwitApplicationFactory<Program>>, IDisposable
{
    private readonly MinitwitApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public MinitwitUserInteraction(MinitwitApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        using (var setupScope = _factory.Services.CreateScope())
        {
            var context = setupScope.ServiceProvider.GetService<MinitwitContext>();

            var createUserTask = _client.CreateTestUserAsync("UserInteractionUser1");
            createUserTask.Wait();
        }
    }

    [Fact]
    public async Task LoginAndLogout_WithCorrectCredentials_ReturnsCorrectMessages()
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string> { { "username", "UserInteractionUser1" }, { "password", "default" } });

        var response = await _client.PostAsync("/login", content);
        var responseText = await response.Content.ReadAsStringAsync();

        Assert.Contains("You were logged in", responseText);

        var logoutResponse = await _client.PostAsync("/logout", new FormUrlEncodedContent(Enumerable.Empty<KeyValuePair<string, string>>()));
        var logoutResponseText = await logoutResponse.Content.ReadAsStringAsync();

        Assert.Contains("You were logged out", logoutResponseText);
    }

    public void Dispose()
    {
        using (var cleanUpScope = _factory.Services.CreateScope())
        {
            var context = cleanUpScope.ServiceProvider.GetService<MinitwitContext>();

            if (context is null)
                return;

            context.Users.RemoveRange(context.Users.Where(x => x.Username.StartsWith("UserInteraction")).ToList());
            context.SaveChanges();
        }
    }
}
