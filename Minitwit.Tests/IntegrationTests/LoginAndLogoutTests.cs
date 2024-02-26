using Microsoft.AspNetCore.Mvc.Testing;
using Minitwit.Tests.Utils;

namespace Minitwit.Tests.IntegrationTests;

public class MinitwitUserInteraction : IClassFixture<MinitwitApplicationFactory<Program>>
{
    private readonly MinitwitApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public MinitwitUserInteraction(MinitwitApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        var createUserTask = _client.CreateTestUserAsync();
        createUserTask.Wait();
    }


}
