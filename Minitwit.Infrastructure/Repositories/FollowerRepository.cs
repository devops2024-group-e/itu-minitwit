using Microsoft.EntityFrameworkCore;
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
        Task<bool> addingFollower = this.AddFollowerAsync(whoId, whomId);

        addingFollower.Wait(); // Wait for the result to get back
        bool followerIsAdded = addingFollower.Result;

        return followerIsAdded;
    }

    public bool RemoveFollower(int whoId, int whomId)
    {
        Task<bool> removingFollower = this.RemoveFollowerAsync(whoId, whomId);

        removingFollower.Wait();
        bool followerIsRemoved = removingFollower.Result;

        return followerIsRemoved;
    }


    public bool IsFollowing(int whoId, int whomId)
    {
        Task<bool> gettingIsFollowing = this.IsFollowingAsync(whoId, whomId);

        gettingIsFollowing.Wait();
        bool isFollowing = gettingIsFollowing.Result;

        return isFollowing;
    }

    public List<string> GetCurrentUserFollows(int whoId, int no)
    {
        Task<List<string>> gettingCurrentUserFollowers = this.GetCurrentUserFollowsAsync(whoId, count: no);

        gettingCurrentUserFollowers.Wait();

        return gettingCurrentUserFollowers.Result;
    }

    public async Task<bool> AddFollowerAsync(int whoId, int whomId)
    {
        _context.Followers.Add(new Follower
        {
            WhoId = whoId,
            WhomId = whomId
        });

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception _)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> RemoveFollowerAsync(int whoId, int whomId)
    {
        var follower = await _context.Followers.FirstOrDefaultAsync(x => x.WhoId == whoId && x.WhomId == whomId);

        if (follower is null)
            return true;

        _context.Followers.Remove(follower);

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

    public async Task<bool> IsFollowingAsync(int whoId, int whomId)
        => await _context.Followers.AnyAsync(x => x.WhoId == whoId && x.WhomId == whomId);

    public Task<List<string>> GetCurrentUserFollowsAsync(int whoId, int count)
    => Task.Run(() =>
    {
        return (from user in _context.Users
                join follower in _context.Followers
                on user.UserId equals follower.WhomId
                where follower.WhoId == whoId
                select user.Username).Take(count).ToList();
    });

}
