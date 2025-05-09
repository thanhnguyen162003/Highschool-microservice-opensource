namespace Application.Common.Models.AIModels;

public class DocumentAISetting
{
    public string Authorization { get; set; } = null!;
    public string Connection { get; set; } = null!;
    public string BotId { get; set; } = null!;
    public bool Stream { get; set; }
    public string User { get; set; } = null!;
    public string API { get; set; } = null!;
}
