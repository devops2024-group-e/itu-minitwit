using System;
using System.Collections.Generic;

namespace Minitwit.Models;

public partial class Message
{
    public long MessageId { get; set; }

    public long AuthorId { get; set; }

    public string Text { get; set; } = null!;

    public long? PubDate { get; set; }

    public long? Flagged { get; set; }
}
