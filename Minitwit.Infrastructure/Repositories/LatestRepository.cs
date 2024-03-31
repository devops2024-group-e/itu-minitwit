using Minitwit.Infrastructure.Models;

namespace Minitwit.Infrastructure.Repositories;

public class LatestRepository : ILatestRepository
{
    private readonly MinitwitContext _context;

    public LatestRepository(MinitwitContext context)
    {
        _context = context;
    }

    public bool AddLatest(int CommandId)
    {
        _context.Latests.Add(new Latest
        {
            CommandId = CommandId
        });
        try
        {
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }

    public int GetLatest()
    {
        var content = (from l in _context.Latests
                       orderby l.Id descending
                       select l.CommandId).Take(1).ToList().FirstOrDefault(-1);

        return content;
    }
}
