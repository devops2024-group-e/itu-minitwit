namespace Minitwit.Tests.Utils;

public static class MinitwitClientExtensions
{
    private const string REGISTER_ENDPOINT = "/register";
    private const string LOGIN_ENDPOINT = "/login";
    private const string ADD_MESSAGE_ENDPOINT = "/add_message";

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

    public static async Task<(bool Success, string? Content)> LoginUserAsync(this HttpClient client, string username, string password)
    {
        var formValues = new Dictionary<string, string> {
            { "username", username },
            { "password", password }
        };
        var content = new FormUrlEncodedContent(formValues);

        var response = await client.PostAsync(LOGIN_ENDPOINT, content);
        bool success = response.IsSuccessStatusCode;

        var responseText = await response.Content.ReadAsStringAsync();

        return (success, responseText);
    }

    public static async Task<string> LogoutUserAsync(this HttpClient client)
    {
        var logoutResponse = await client.PostAsync("/logout", new FormUrlEncodedContent(Enumerable.Empty<KeyValuePair<string, string>>()));
        return await logoutResponse.Content.ReadAsStringAsync();
    }

    public static async Task<bool> AddMessageAsync(this HttpClient client, string message)
    {
        var formValues = new Dictionary<string, string> {
            {"text", message}
        };
        var content = new FormUrlEncodedContent(formValues);

        var response = await client.PostAsync(ADD_MESSAGE_ENDPOINT, content);

        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Gets the timeline page. If the user string is omitted then it will navigate to the "/" timeline
    /// </summary>
    /// <param name="client">The httpclient to perform the action on</param>
    /// <param name="user">User to navigate to. If omitted then it navigates</param>
    /// <returns></returns>
    public static async Task<string> GetPageAsync(this HttpClient client, string page = "")
    {
        var response = await client.GetAsync($"/{page}");
        return await response.Content.ReadAsStringAsync();
    }
}
