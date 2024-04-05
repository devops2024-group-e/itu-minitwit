using Minitwit.Infrastructure.Models;
using Minitwit.ViewModels;

namespace Minitwit.Utils;

/// <summary>
/// Contains extensions methods to convert models to view models
/// </summary>
public static class DTOConvertExtensions
{
    /// <summary>
    /// Converts a collection of MessageAuthors to a collection of MessageViewModel
    /// </summary>
    /// <param name="messages">The MessageAuthor model to convert to a view model</param>
    /// <returns>A list of MessageViewModels</returns>
    public static List<MessageViewModel> ConvertToViewModel(this IEnumerable<MessageAuthor> messages)
        => messages.Select(x => x.ConvertToViewModel()).ToList();

    /// <summary>
    /// Converts an instance of MessageAuthor to an instance of MessageViewModel
    /// </summary>
    /// <param name="message">The model to convert to a viewmodel</param>
    /// <returns>A new instance of MessageViewModel equivalent to MessageAuthor</returns>
    public static MessageViewModel ConvertToViewModel(this MessageAuthor message)
        => new MessageViewModel
        {
            MessageId = message.Message.MessageId,
            Author = message.Author.ConvertToAuthorViewModel(),
            Text = message.Message.Text,
            PubDate = new DateTime(message.Message.PubDate ?? long.MinValue),
            Flagged = message.Message.Flagged

        };

    /// <summary>
    /// Converts an instance of User to a AuthorViewModel
    /// </summary>
    /// <param name="user">The model to convert to a viewmodel</param>
    /// <returns>A new instance of a AuthorViewModel</returns>
    public static AuthorViewModel ConvertToAuthorViewModel(this User user)
        => new AuthorViewModel
        {
            Id = user.UserId,
            Name = user.Username,
            Email = user.Email
        };
}
