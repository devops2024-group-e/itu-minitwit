namespace Minitwit.Utils;

public static class AuthExtensions
{
    // The key where the authenticated user is registered
    private const string AUTH_SESSION_KEY = "user_id";

    /// <summary>
    /// Checks if a user is authenticated
    /// </summary>
    /// <param name="session">The session to check that a user is registered as authenticated</param>
    /// <returns>True if a user is authenticated, false if not</returns>
    public static bool IsAuthenticated(this ISession session)
        => session.TryGetValue(AUTH_SESSION_KEY, out byte[]? bytes);

    /// <summary>
    /// Gets the user id of the authenticated user
    /// </summary>
    /// <param name="session">Session to get the user id from</param>
    /// <exception cref="KeyNotFoundException">If the user is not authenticated</exception>
    /// <returns>The user id of the authenticated user</returns>
    public static int GetUserId(this ISession session)
    {
        int userId = session.GetInt32(AUTH_SESSION_KEY) ?? throw new KeyNotFoundException($"Could not get {AUTH_SESSION_KEY} from session");

        return userId;
    }

}
