namespace Minitwit.Infrastructure.Repositories;

public interface ILatestRepository
{
    bool AddLatest(int CommandId);

    int GetLatest();
}
