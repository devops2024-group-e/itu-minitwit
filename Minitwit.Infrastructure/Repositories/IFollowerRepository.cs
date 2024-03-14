namespace Minitwit.Infrastructure.Repositories;

public interface IFollowerRepository
{
    bool AddFollower(int whoId, int whomId);

    Task<bool> AddFollowerAsync(int whoId, int whomId);

    bool RemoveFollower(int whoId, int whomId);

    Task<bool> RemoveFollowerAsync(int whoId, int whomId);

    bool IsFollowing(int whoId, int whomId);

    Task<bool> IsFollowingAsync(int whoId, int whomId);

    List<string> GetCurrentUserFollows(int whoId, int no);

    Task<List<string>> GetCurrentUserFollowsAsync(int whoId, int count);

}
