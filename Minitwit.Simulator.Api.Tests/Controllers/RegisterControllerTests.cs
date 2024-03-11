using System.Net.Http.Headers;
using MinitwitSimulatorAPI;
using static System.Net.HttpStatusCode;

namespace Minitwit.Simulator.Api.Tests.Controllers;

[Collection(nameof(SequentialControllerTestCollectionDefinition))]
public class RegisterControllerTests
{
    private const string USERNAME_PREFIX = "RegisterApi";
    private readonly MinitwitSimulatorApiApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public RegisterControllerTests(MinitwitSimulatorApiApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "c2ltdWxhdG9yOnN1cGVyX3NhZmUh");
    }

    [Theory]
    [InlineData($"{USERNAME_PREFIX}a", "a@a.a", "a", 1)]
    [InlineData($"{USERNAME_PREFIX}b", "b@b.b", "b", 5)]
    [InlineData($"{USERNAME_PREFIX}c", "c@c.c", "c", 6)]
    public async Task Register_WithValidForm_ReturnsNoContentStatusCode(string username, string email, string password, int latest)
    {
        var response = await _client.RegisterUserAsync(username, email, password, latest: latest);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(NoContent, response.StatusCode);
    }
}
