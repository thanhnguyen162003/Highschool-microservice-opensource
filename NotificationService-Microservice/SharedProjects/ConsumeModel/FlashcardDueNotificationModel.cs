
namespace SharedProjects.ConsumeModel
{
    public class FlashcardDueNotificationModel
    {
        public Guid UserId { get; set; }
        public Guid FlashcardId { get; set; }
        public string FlashcardSlug { get; set; } = null!;
        public string FlashcardName { get; set; } = null!;
        public int DueContentCount { get; set; }
    }
}
