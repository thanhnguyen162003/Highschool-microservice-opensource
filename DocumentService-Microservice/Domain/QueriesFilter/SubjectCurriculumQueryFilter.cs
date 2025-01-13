namespace Domain.QueriesFilter;

public class SubjectCurriculumQueryFilter
{
    public string? Search { get; set; }
   
    public Guid? SortCurriculum { get; set; }
   
    public int PageSize { get; set; }
    
    public int PageNumber { get; set; }

}