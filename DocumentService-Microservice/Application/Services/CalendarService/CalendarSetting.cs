namespace Application.Services.CalendarService;

public class CalendarSetting
{
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string Scope { get; set; } = null!;
    public string RedirectUri { get; set; } = null!;
    public string AuthUrl { get; set; } = null!;
    public string TokenUri { get; set; } = null!;
    public string AuthCertUri { get; set; } = null!;
}