using Minitwit.Infrastructure.Models;

namespace Minitwit.Infrastructure.Repositories;

/// <summary>
/// Represents operations to get, delete or change Messages from a data store
/// </summary>
public interface IMessageRepository
{
    /// <summary>
    /// Adds a message with an author id
    /// </summary>
    /// <param name="text">The text of the message</param>
    /// <param name="authorId">The user id of the author of the message</param>
    /// <returns>True if the message is added, false if not</returns>
    Task<bool> AddMessageAsync(string text, int authorId);

    /// <summary>
    /// Gets all messages
    /// </summary>
    /// <param name="no">The maximum nomber of messages to return</param>
    /// <returns>Returns a list of all messages with a maximum of no</returns>
    Task<List<MessageAuthor>> GetMessagesAsync(int no);

    /// <summary>
    /// Get user specific messages
    /// </summary>
    /// <param name="profileUser">The user who posted the messages</param>
    /// <param name="no">The maximum amount of messages to return</param>
    /// <returns>Returns a list of all messages with a maximum of no</returns>
    Task<List<MessageAuthor>> GetUserSpecificMessagesAsync(User profileUser, int no);

    /// <summary>
    /// Gets the current users posts
    /// </summary>
    /// <param name="currentUserId">Id of the current user</param>
    /// <param name="no">The maximum number of messages to return</param>
    /// <returns>Returns a list of all messages with a maximum of no</returns>
    Task<List<MessageAuthor>> GetCurrentUserSpecificMessagesAsync(int currentUserId, int no);
}
