using Microsoft.EntityFrameworkCore;
using Minitwit.Infrastructure.Models;

namespace Minitwit.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MinitwitContext _context;

    public UserRepository(MinitwitContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserAsync(string username)
        => await _context.Users.SingleOrDefaultAsync(x => x.Username == username);


    public async Task<User?> GetUserAsync(int currentUserId)
        => await _context.Users.SingleAsync(x => x.UserId == currentUserId);

    public async Task<bool> DoesUserExistAsync(string username)
        => await _context.Users.AnyAsync(x => x.Username == username);

    public async Task<bool> AddUserAsync(string username, string email, string password)
    {
        User user = new User // We should use another type to represent the model the database
        {
            Username = username,
            Email = email,
            PwHash = password
        };
        {
            Username = username,
            Email = email,
            PwHash = password
        };

        _context.Users.Add(user);
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
}
