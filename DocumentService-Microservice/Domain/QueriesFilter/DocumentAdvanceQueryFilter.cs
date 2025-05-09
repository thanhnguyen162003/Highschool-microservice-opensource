using Domain.Enums;
using System.Web;

namespace Domain.QueriesFilter;

public class DocumentAdvanceQueryFilter
{
    public string? Search { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public bool? SortPopular { get; set; }   
    public Guid? SchoolId { get; set; }
    public List<Guid>? SubjectIds { get; set; }
    public int? Semester { get; set; }
    public int? DocumentYear { get; set; }
    public int? ProvinceId { get; set; }
    public string? Category { get; set; }
    public List<Guid>? CurriculumIds { get; set; }
    public List<Guid>? MasterSubjectIds { get; set; }
}