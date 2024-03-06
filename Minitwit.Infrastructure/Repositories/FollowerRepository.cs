using Minitwit.Infrastructure.Models;

namespace Minitwit.Infrastructure.Repositories;

public class FollowerRepository : IFollowerRepository
{
    private readonly MinitwitContext _context;

    public FollowerRepository(MinitwitContext context)
    {
        _context = context;
    }

    public bool AddFollower(int whoId, int whomId)
    {
        _context.Followers.Add(new Follower
        {
            WhoId = whoId,
            WhomId = whomId
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

    public bool RemoveFollower(int whoId, int whomId)
    {
        _context.Followers.Remove(new Follower
        {
            WhoId = whoId,
            WhomId = whomId
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