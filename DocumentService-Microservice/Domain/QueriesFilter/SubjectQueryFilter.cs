
using System.ComponentModel.DataAnnotations;

namespace Domain.QueriesFilter;

public class SubjectQueryFilter
{
    public string? Search { get; set; }
   
    public string? Class { get; set; }
   
    public int PageSize { get; set; }
    
    public int PageNumber { get; set; }
    
}