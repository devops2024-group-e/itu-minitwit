using System.ComponentModel.DataAnnotations;

namespace Minitwit.Models;
public class MessageDTO
{
    public int MessageId { get; set; }

    public AuthorDTO? Author { get; set; }

    public string Text { get; set; } = null!;

    [DisplayFormat(DataFormatString = "dd/MM/yyyy HH:mm")]
    public DateTime? PubDate { get; set; }

    public int? Flagged { get; set; }
}
