namespace Minitwit.Infrastructure.Repositories;


/// <summary>
/// Represents operations to get, add, delete or change Followers from a data store
/// </summary>
public interface IFollowerRepository
{

    /// <summary>
    /// Adds a new follower relateion between two user ids
    /// </summary>
    /// <param name="whoId">Id of the user that wants to follow</param>
    /// <param name="whomId">Id of the user being followed</param>
    /// <returns>True if the relation has been added, false if not</returns>
    Task<bool> AddFollowerAsync(int whoId, int whomId);

    /// <summary>
    /// Removes a follower relation
    /// </summary>
    /// <param name="whoId">Id of the user that wants to unfollow</param>
    /// <param name="whomId">Id of the user being unfollowed</param>
    /// <returns>True if the relation has been removed</returns>
    Task<bool> RemoveFollowerAsync(int whoId, int whomId);

    /// <summary>
    /// Checks that the the user with the whoId is following the user with whomId
    /// </summary>
    /// <param name="whoId">The id of the user who should be following</param>
    /// <param name="whomId">The id of the user who should be followed</param>
    /// <returns>True if the relation is present, false if not</returns>
    Task<bool> IsFollowingAsync(int whoId, int whomId);

    /// <summary>
    /// Gets a list of the users who the whoid is currently following
    /// </summary>
    /// <param name="whoId">The user id to get the follower list from</param>
    /// <param name="count">The maximum amount of followers to get back</param>
    /// <returns>Returns a list of user who the whoid follows. The list has a length > count</returns>
    Task<List<string>> GetCurrentUserFollowsAsync(int whoId, int count);

}
