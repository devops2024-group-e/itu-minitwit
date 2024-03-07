namespace Minitwit.Simulator.Api.Tests;

public static class MinitwtiApiHttpClientExtensions
{
    public static async Task<HttpResponseMessage> RegisterUserAsync(this HttpClient client, string username, string email, string password, int latest)
        => await client.PostAsJsonAsync($"/register?latest={latest}", new
        {
            Username = username,
            Email = email,
            Pwd = password
        });

    public static async Task<HttpResponseMessage> CreateMessageAsync(this HttpClient client, string content, string username, int latest)
        => await client.PostAsJsonAsync($"/msgs/{username}?latest={latest}", new { Content = content });

    public static async Task<HttpResponseMessage> GetLatestMessagesAsync(this HttpClient client, int messageCount, int latest)
        => await client.GetAsync($"/msgs?latest={latest}&no={messageCount}");

    public static async Task<HttpResponseMessage> GetLatestUserMessagesAsync(this HttpClient client, string username, int messageCount, int latest)
        => await client.GetAsync($"/msgs/{username}?latest={latest}&no={messageCount}");

    public static async Task<HttpResponseMessage> FollowUserAsync(this HttpClient client, string currentUser, string usernameToFollow, int latest)
        => await client.PostAsJsonAsync($"/fllws/{currentUser}?latest={latest}", new { Follow = usernameToFollow });

    public static async Task<HttpResponseMessage> UnfollowUserAsync(this HttpClient client, string currentUser, string usernameToUnFollow, int latest)
        => await client.PostAsJsonAsync($"/fllws/{currentUser}?latest={latest}", new { Unfollow = usernameToUnFollow });
}
