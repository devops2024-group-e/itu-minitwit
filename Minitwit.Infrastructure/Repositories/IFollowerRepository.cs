namespace Minitwit.Infrastructure.Repositories;

public interface IFollowerRepository
{
    bool AddFollower(int whoId, int whomId);

    bool RemoveFollower(int whoId, int whomId);

    bool IsFollowing(int whoId, int whomId);

    List<string> GetCurrentUserFollows(int whoId, int no);
}