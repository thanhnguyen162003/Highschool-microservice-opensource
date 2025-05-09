using Domain.Enums;

namespace Application.Common.Models.DocumentModel
{
    public class CreateDocumentRequestModel
    {
        public string? DocumentName { get; set; }
        public string? DocumentDescription { get; set; }
        public Guid? SchoolId { get; set; }
        public Guid? CurriculumId { get; set; }
        public Guid? SubjectId { get;set; }
        public int? Semester { get; set; }
        public int? DocumentYear { get; set; }
    }
}
