namespace Application.Common.Models;

public class NotificationModel
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Type { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? LinkDetail { get; set; }
}