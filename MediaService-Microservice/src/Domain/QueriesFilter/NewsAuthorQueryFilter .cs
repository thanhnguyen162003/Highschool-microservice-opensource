namespace Domain.QueriesFilter;

public class NewsAuthorQueryFilter
{
    public string? Search { get; set; }
   
    public int PageSize { get; set; }
    
    public int PageNumber { get; set; }
    public string? Direction { get; set; }
}
