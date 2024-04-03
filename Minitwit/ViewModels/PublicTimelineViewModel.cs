using Minitwit.Models;

namespace Minitwit.ViewModels
{
    public class PublicTimelineViewModel
    {
        public required ICollection<MessageDTO> Messages { get; set; }
    }
}
