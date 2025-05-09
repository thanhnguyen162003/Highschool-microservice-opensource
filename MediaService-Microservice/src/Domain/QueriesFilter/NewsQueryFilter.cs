namespace Domain.QueriesFilter;

public class NewsQueryFilter
{
    public Guid? NewsTagId { get; set; }
    public string? Location { get; set; }
    public string? Search { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public string? Sort { get; set; }
    public string? Direction { get; set; }
}
