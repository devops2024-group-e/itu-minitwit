namespace Minitwit.Infrastructure.Repositories;

public interface ILatestRepository
{
    bool AddLatest(int CommandId);

    Task<bool> AddLatestAsync(int commandId);

    int GetLatest();

    Task<int> GetLatestAsync();
}
