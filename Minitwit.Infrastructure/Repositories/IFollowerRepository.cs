namespace Minitwit.Infrastructure.Repositories;

public interface IFollowerRepository
{
    bool AddFollower(int whoId, int whomId);

    bool RemoveFollower(int whoId, int whomId);
}