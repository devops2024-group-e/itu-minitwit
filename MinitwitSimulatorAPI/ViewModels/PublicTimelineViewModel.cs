using MinitwitSimulatorAPI.Models;

namespace MinitwitSimulatorAPI.ViewModels
{
    public class PublicTimelineViewModel
    {
        public required ICollection<MessageAuthor> Messages { get; set; }
    }
}
