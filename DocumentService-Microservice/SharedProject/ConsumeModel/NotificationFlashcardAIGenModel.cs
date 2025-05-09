using System;

namespace SharedProject.ConsumeModel;

public class NotificationFlashcardAIGenModel
{
    public Guid UserId { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public Guid FlashcardId { get; set; }
}
