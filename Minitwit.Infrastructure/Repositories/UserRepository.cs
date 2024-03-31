using System.Diagnostics;
using Minitwit.Infrastructure.Models;

namespace Minitwit.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MinitwitContext _context;

    public UserRepository(MinitwitContext context)
    {
        _context = context;
    }

    public User? GetUser(string username)
    {
        return _context.Users.SingleOrDefault(x => x.Username == username);
    }

    public User GetUser(int currentUserId)
    {
        return _context.Users.Single(x => x.UserId == currentUserId);
    }

    public bool DoesUserExist(string username)
    {
        return _context.Users.Any(x => x.Username == username);
    }

    public bool AddUser(string username, string email, string password)
    {
        User user = new User // We should use another type to represent the model the database
        {
            Username = username,
            Email = email,
            PwHash = password
        };

        _context.Users.Add(user);
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

}
