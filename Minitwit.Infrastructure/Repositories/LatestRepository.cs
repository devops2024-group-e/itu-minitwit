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
        } catch(Exception e)
        {
            return false;
        }
        return true;
    }
}