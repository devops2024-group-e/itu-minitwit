using System;
using System.Collections.Generic;

namespace Minitwit.Infrastructure.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int AuthorId { get; set; }

    public string Text { get; set; } = null!;

    public long? PubDate { get; set; }

    public int? Flagged { get; set; }
}
