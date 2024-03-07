using Microsoft.AspNetCore.Mvc.Testing;

namespace Minitwit.Tests.IntegrationTests;

public class RegisterTests : IClassFixture<MinitwitApplicationFactory<Program>>, IDisposable
{

    private readonly MinitwitApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public RegisterTests(MinitwitApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    private IDictionary<string, string> CreateRegisterFormData(string username, string password, string? password2 = null, string? email = null)
    {
        if (string.IsNullOrEmpty(password2))
            password2 = password;
        if (string.IsNullOrEmpty(email))
            email = $"{username}@example.com";

        return new Dictionary<string, string>{
            { "username", username },
            { "password", password },
            { "password2", password2 },
            { "email", email }
        };
    }

    [Theory()]
    [InlineData("user1", "default", "default", "", "You were successfully registered and can login now")]
    [InlineData("", "default", "", "", "You have to enter a username")]
    [InlineData("meh", "x", "y", "", "The two passwords do not match")]
    [InlineData("meh", "foo", "foo", "broken", "You have to enter a valid email address")]
    public async Task Register_WithCombinationsOfUsernamesAndPasswords_ReturnsCorrectPopupMessage(string username, string password, string password2, string email, string expectedMessage)
    {
        // Arrange
        var content = new FormUrlEncodedContent(CreateRegisterFormData(username, password, password2, email));

        // Act
        var resp = await _client.PostAsync("/register", content);
        var responseText = await resp.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains(expectedMessage, responseText);
    }

    [Fact]
    public async Task Register_WithSameUsername_ReturnNameAlreadyTakenMessage()
    {
        // Arrange
        string username = "user2";
        string password = "default";
        var content = new FormUrlEncodedContent(CreateRegisterFormData(username, password, password, ""));

        // Act
        var resp = await _client.PostAsync("/register", content);
        var responseText = await resp.Content.ReadAsStringAsync();
        Assert.Contains("You were successfully registered and can login now", responseText);

        // Create the same user again
        var response = await _client.PostAsync("/register", content);
        var result = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("The username is already taken", result);

    }

    public void Dispose()
    {
        using (var cleanUpScope = _factory.Services.CreateScope())
        {
            var context = cleanUpScope.ServiceProvider.GetService<MinitwitContext>();

            if (context is null)
                return;

            context.Users.RemoveRange(context.Users.ToList());
            context.SaveChanges();
        }
    }
}
