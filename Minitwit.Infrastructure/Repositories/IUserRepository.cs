using Minitwit.Infrastructure.Models;

namespace Minitwit.Infrastructure.Repositories;

public interface IUserRepository
{
    User? GetUser(string username);

    User GetUser(int currentUserId);

    bool DoesUserExist(string username);

    public bool AddUser(string username, string email, string password);
}