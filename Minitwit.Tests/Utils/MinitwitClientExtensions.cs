namespace Minitwit.Tests.Utils;

public static class MinitwitClientExtensions
{
    private const string REGISTER_ENDPOINT = "/register";
    private const string TEST_PASSWORD = "default";

    public static async Task CreateTestUserAsync(this HttpClient client, string username)
    {
        if (string.IsNullOrEmpty(username))
            throw new ArgumentException("Username cannot be a null or empty string");

        var reqContent = new Dictionary<string, string>{
            { "username", username },
            { "password", TEST_PASSWORD },
            { "password2", TEST_PASSWORD },
            { "email", $"{username}@example.com" }
        };
        var content = new FormUrlEncodedContent(reqContent);

        // Act
        var resp = await client.PostAsync(REGISTER_ENDPOINT, content);

        if (!resp.IsSuccessStatusCode)
            throw new Exception("Could not create test user");
    }
}
