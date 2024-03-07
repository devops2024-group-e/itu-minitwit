using Minitwit.Infrastructure.Models;

namespace Minitwit.Infrastructure.Repositories;

public interface IMessageRepository
{
    bool AddMessage(string text, int authorId);

    List<MessageAuthor> GetMessages();

    List<MessageAuthor> GetUserSpecificMessages(User profileUser);

    List<MessageAuthor> GetCurrentUserSpecificMessages(int currentUserId);
}