namespace SharedProject.Models;

public class NotificationUserModel
{
    public string? UserId { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public DateTime? CreatedAt { get; set; }
}