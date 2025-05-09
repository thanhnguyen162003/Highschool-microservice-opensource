using Domain.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Application.Common.Models.DocumentModel
{
    public class UpdateDocumentRequestModel
    {
        public string? DocumentName { get; set; }
        public string? DocumentDescription { get; set; }
        public int? DocumentYear { get; set; }      
        public Guid? SchoolId { get; set; }
        public Guid? SubjectId { get; set; }
        public Guid? CurriculumId { get; set; }
        public int? Semester { get; set; }
        //NOT USE IN MAPPING, SUPPOSELY
        [BindNever]
        public string? DocumentFileName { get; set; }
        [BindNever]
        public bool? IsDownloaded { get; set; }
    }
}
