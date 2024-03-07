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
        var follower = _context.Followers.FirstOrDefault(x => x.WhoId == whoId && x.WhomId == whomId);
        _context.Followers.Remove(follower);
        try
        {
            _context.SaveChanges();
        } catch(Exception e)
        {
            return false;
        }
        return true;
    }

    public bool IsFollowing(int whoId, int whomId)
    {
        var isFollowing = _context.Followers
                                .Any(x => x.WhoId == whoId && x.WhomId == whomId);
        return isFollowing;
    }

    public List<string> GetCurrentUserFollows(int whoId, int no)
    {
        var follows = (from user in _context.Users
                       join follower in _context.Followers
                       on user.UserId equals follower.WhomId
                       where follower.WhoId == whoId
                       select user.Username).Take(no).ToList();
        
        return follows;
    }
}