namespace Minitwit.Infrastructure.Repositories;


/// <summary>
/// Represents operations to get or add Latest ids from a data store
/// </summary>
public interface ILatestRepository
{
    /// <summary>
    /// Adds the latest command id
    /// </summary>
    /// <param name="commandId">Command id of the latests call from the simulator</param>
    /// <returns>True if the id is added</returns>
    Task<bool> AddLatestAsync(int commandId);

    /// <summary>
    /// Gets the latest id
    /// </summary>
    /// <returns>Returns the most recent Id</returns>
    Task<int> GetLatestAsync();
}
