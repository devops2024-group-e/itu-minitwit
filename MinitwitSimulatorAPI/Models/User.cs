using System;
using System.Collections.Generic;

namespace MinitwitSimulatorAPI.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PwHash { get; set; } = null!;
}
