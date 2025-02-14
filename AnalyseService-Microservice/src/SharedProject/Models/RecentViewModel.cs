namespace SharedProject.Models;

public class RecentViewModel
{
    public Guid UserId { get; set; }
    public string? TypeDocument { get; set; }
    public Guid IdDocument { get; set; }
    public string? SlugDocument { get; set; }
    public string? DocumentName { get; set; }
    public DateTime? Time { get; set; }
}
