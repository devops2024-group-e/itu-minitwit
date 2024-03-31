using Minitwit.Infrastructure.Models;

namespace Minitwit.Infrastructure.Repositories;

public interface IMessageRepository
{
    bool AddMessage(string text, int authorId);

    List<MessageAuthor> GetMessages(int no);

    List<MessageAuthor> GetUserSpecificMessages(User profileUser, int no);

    List<MessageAuthor> GetCurrentUserSpecificMessages(int currentUserId, int no);
}
