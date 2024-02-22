using MinitwitSimulatorAPI.Models;

namespace MinitwitSimulatorAPI.ViewModels;

public class TimelineViewModel
{
    public string? CurrentUsername { get; set; }
    public Profile? Profile { get; set; }
    public required ICollection<MessageAuthor> Messages { get; set; }
}

public class Profile
{
    public required string Username { get; set; }
    public int UserId { get; set; }
    public bool IsFollowing { get; set; }
    public bool IsMe { get; set; }
}
