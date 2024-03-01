using Minitwit.Infrastructure.Models;

namespace Minitwit.Infrastructure.Repositories;

public class UserRepository
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

}