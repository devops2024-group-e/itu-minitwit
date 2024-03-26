using Minitwit.Infrastructure.Models;

namespace Minitwit.Infrastructure.Repositories;

/// <summary>
/// Represents operations to get, delete or change Users from a data store
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets the user by username
    /// </summary>
    /// <param name="username">Username to get the user from</param>
    /// <returns></returns>
    Task<User?> GetUserAsync(string username);

    /// <summary>
    /// Gets the user by id
    /// </summary>
    /// <param name="currentUserId">The id of the user to get</param>
    /// <returns>
    /// The user with particular id.
    /// If null then it means it could not find the user
    /// </returns>
    Task<User?> GetUserAsync(int currentUserId);

    /// <summary>
    /// Checks if a user with the given username exists in the datastore
    /// </summary>
    /// <param name="username">The username to look for</param>
    /// <returns>True if a user with the username exists, and false if not</returns>
    Task<bool> DoesUserExistAsync(string username);

    /// <summary>
    /// Adds a user to the datastore
    /// </summary>
    /// <param name="username">Username of the user</param>
    /// <param name="email">Email of the user</param>
    /// <param name="password">Password of the user</param>
    /// <returns>True if the user is added succesfully</returns>
    Task<bool> AddUserAsync(string username, string email, string password);
}
