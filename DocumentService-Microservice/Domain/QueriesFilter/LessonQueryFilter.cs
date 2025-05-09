namespace Domain.QueriesFilter;

public class LessonQueryFilter
{
    public string? Search { get; set; }
   
    public int PageSize { get; set; }
    
    public int PageNumber { get; set; }
}