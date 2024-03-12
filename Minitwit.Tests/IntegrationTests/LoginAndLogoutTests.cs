using Microsoft.AspNetCore.Mvc.Testing;
using Minitwit.Infrastructure;
using Minitwit.Tests.Utils;

namespace Minitwit.Tests.IntegrationTests;

public class LoginAndLogoutTests : IClassFixture<MinitwitApplicationFactory<Program>>, IDisposable
{
    private readonly MinitwitApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public LoginAndLogoutTests(MinitwitApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        using (var setupScope = _factory.Services.CreateScope())
        {
            var context = setupScope.ServiceProvider.GetService<MinitwitContext>();

            var createUserTask = _client.CreateTestUserAsync("LoginAndLogoutUser1");
            createUserTask.Wait();
        }
    }

    private IDictionary<string, string> CreateLoginFormData(string username, string password)
        => new Dictionary<string, string> { { "username", username }, { "password", password } };


    [Fact]
    public async Task LoginAndLogout_WithCorrectCredentials_ReturnsCorrectMessages()
    {
        var content = new FormUrlEncodedContent(CreateLoginFormData("LoginAndLogoutUser1", "default"));

        var response = await _client.PostAsync("/login", content);
        var responseText = await response.Content.ReadAsStringAsync();

        Assert.Contains("You were logged in", responseText);

        var logoutResponse = await _client.PostAsync("/logout", new FormUrlEncodedContent(Enumerable.Empty<KeyValuePair<string, string>>()));
        var logoutResponseText = await logoutResponse.Content.ReadAsStringAsync();

        Assert.Contains("You were logged out", logoutResponseText);
    }

    [Theory]
    [InlineData("LoginAndLogoutUser1", "wrongpassword", "Invalid password")]
    [InlineData("LoginAndLogoutUserDoesNotExist", "wrongpassword", "Invalid username")]
    public async Task Login_WrongCredentials_ReturnsCorrectErrorMessage(string username, string password, string expectedMessage)
    {
        // Arrange
        var content = new FormUrlEncodedContent(CreateLoginFormData(username, password));

        // Act
        var response = await _client.PostAsync("/login", content);
        var responseText = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains(expectedMessage, responseText);
    }

    public void Dispose()
    {
        using (var cleanUpScope = _factory.Services.CreateScope())
        {
            var context = cleanUpScope.ServiceProvider.GetService<MinitwitContext>();

            if (context is null)
                return;

            context.Users.RemoveRange(context.Users.Where(x => x.Username.StartsWith("LoginAndLogout")).ToList());
            context.SaveChanges();
        }
    }
}
