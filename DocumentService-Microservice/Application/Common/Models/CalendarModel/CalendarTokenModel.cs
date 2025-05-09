namespace Application.Common.Models.CalendarModel;

public class CalendarTokenModel
{
    public CalendarTokenModel()
    {
        this.CreateAt = DateTime.UtcNow;
    }
    public string access_token { get; set; } 
    public string refresh_token { get; set; }
    public string token_type { get; set; } 
    public string scope { get; set; } 
    public int expires_in { get; set; }
    public DateTime CreateAt { get; set; }

    public bool IsExpired
    {
        get
        {
            return this.CreateAt.AddSeconds(this.expires_in) < DateTime.UtcNow;
        }
    }
}
