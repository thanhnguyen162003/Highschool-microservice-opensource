namespace Domain.QueriesFilter;

public class SchoolQueryFilter
{
    public string? Search { get; set; }

    public string? SearchProvince { get; set; }
    
    public int PageSize { get; set; }
    
    public int PageNumber { get; set; }
}