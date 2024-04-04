using System.ComponentModel.DataAnnotations;

namespace Minitwit.ViewModels;
public class MessageViewModel
{
    public int MessageId { get; set; }

    public AuthorViewModel? Author { get; set; }

    public string Text { get; set; } = null!;

    [DisplayFormat(DataFormatString = "dd/MM/yyyy HH:mm")]
    public DateTime? PubDate { get; set; }

    public int? Flagged { get; set; }
}
