using Minitwit.Infrastructure.Models;

namespace Minitwit.Infrastructure.Repositories;

public interface IUserRepository
{
    User? GetUser(string username);

    User GetUser(int currentUserId);

    bool DoesUserExist(string username);

    bool AddUser(string username, string email, string password);

    Task<User?> GetUserAsync(string username);

    Task<User?> GetUserAsync(int currentUserId);

    Task<bool> DoesUserExistAsync(string username);

    Task<bool> AddUserAsync(string username, string email, string password);
}
