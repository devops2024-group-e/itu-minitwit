﻿using System;
using System.Collections.Generic;

namespace Minitwit.Infrastructure.Models;

public partial class Follower
{
    public int WhoId { get; set; }

    public int WhomId { get; set; }
}
