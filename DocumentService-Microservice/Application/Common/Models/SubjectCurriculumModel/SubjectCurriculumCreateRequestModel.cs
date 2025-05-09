namespace Application.Common.Models.SubjectCurriculumModel;

public class SubjectCurriculumCreateRequestModel
{
    public Guid SubjectId { get; set; }
    public Guid CurriculumId { get; set; }
    public string? SubjectCurriculumName { get; set; }
}