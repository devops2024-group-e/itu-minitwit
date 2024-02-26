namespace Minitwit.Tests.Utils;

public static class MinitwitClientExtensions
{
    private const string REGISTER_ENDPOINT = "/register";
    private const string TEST_USERNAME = "user1";
    private const string TEST_PASSWORD = "default";

    public static async Task CreateTestUserAsync(this HttpClient client)
    {
        var reqContent = new Dictionary<string, string>{
            { "username", TEST_USERNAME },
            { "password", TEST_PASSWORD },
            { "password2", TEST_PASSWORD },
            { "email", $"{TEST_USERNAME}@example.com" }
        };
        var content = new FormUrlEncodedContent(reqContent);

        // Act
        var resp = await client.PostAsync(REGISTER_ENDPOINT, content);

        if (!resp.IsSuccessStatusCode)
            throw new Exception("Could not create test user");
    }
}
