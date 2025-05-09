using Application.Common.Models.SearchModel;
using Domain.Enums;

namespace Application.Common.Models.DocumentModel
{
    public class DocumentResponseModel : SearchResponseModel
    {
        public Guid Id { get; set; }
        public string? DocumentSlug { get; set; }
        public string DocumentName { get; set; } = null!;
        public string? DocumentDescription { get; set; }
        public int? DocumentYear { get; set; }
        public int? View { get; set; }
        public int? Download { get; set; }
        public string? SchoolName { get; set; }
        public bool? IsLike { get; set; } = false!;
        public int? Like { get; set; }
        public DocumentSubjectCurriculumResponseModel? SubjectCurriculum { get; set; }
        public string? Category { get; set; }
        public DocumentMasterSubjectResponseModel? MasterSubject { get; set; }
        public int? Semester { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class DocumentSubjectCurriculumResponseModel
    {
        public Guid SubjectCurriculumId { get; set; }
        public Guid SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public Guid CurriculumId { get; set; }
        public string? CurriculumName { get; set; }
    }

    public class DocumentMasterSubjectResponseModel
    {
        public Guid MasterSubjectId { get; set; }
        public string MasterSubjectName { get; set; } = null!;
        public string MasterSubjectSlug { get; set; } = null!;
    }
}
