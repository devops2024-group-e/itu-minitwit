using System;
using System.Collections.Generic;

namespace Minitwit.Models;

public partial class Follower
{
    public long WhoId { get; set; }

    public long WhomId { get; set; }
}
