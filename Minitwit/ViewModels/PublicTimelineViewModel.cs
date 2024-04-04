namespace Minitwit.ViewModels
{
    public class PublicTimelineViewModel
    {
        public required ICollection<MessageViewModel> Messages { get; set; }
    }
}
