using Minitwit.Infrastructure.Models;

namespace Minitwit.Infrastructure.Repositories;

public class LatestRepository : ILatestRepository
{
    private readonly MinitwitContext _context;

    public LatestRepository(MinitwitContext context)
    {
        _context = context;
    }

    public async Task<bool> AddLatestAsync(int commandId)
    {
        _context.Latests.Add(new Latest
        {
            CommandId = commandId
        });

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public int GetLatest()
    {
        Task<int> gettingLatest = this.GetLatestAsync();

        gettingLatest.Wait();
        return gettingLatest.Result;
    }

    public Task<int> GetLatestAsync()
        => Task.Run(() =>
        {
            return (from l in _context.Latests
                    orderby l.Id descending
                    select l.CommandId).Take(1).ToList().FirstOrDefault(-1);
        });
}
