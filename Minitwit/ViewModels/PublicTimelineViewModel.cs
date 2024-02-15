using Minitwit.Models;

namespace Minitwit.ViewModels
{
    public class PublicTimelineViewModel
    {
        public required ICollection<MessageAuthor> Messages { get; set; }
    }
}
