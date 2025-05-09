namespace Application.Common.Models.RecentViewModel;

public class RecentViewResponseModel
{
    public List<RecentViewItem> Items { get; set; } = new();
}

public class RecentViewItem
{
    public Guid IdDocument { get; set; }
    public string? DocumentName { get; set; }
    public string? SlugDocument { get; set; }
    public string? TypeDocument { get; set; }
    public DateTime? Time { get; set; }
}
