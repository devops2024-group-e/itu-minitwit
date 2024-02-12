using System;
using System.Collections.Generic;

namespace itu_new_minitwit.Models;

public partial class Message
{
    public long MessageId { get; set; }

    public long AuthorId { get; set; }

    public byte[] Text { get; set; } = null!;

    public long? PubDate { get; set; }

    public long? Flagged { get; set; }
}
