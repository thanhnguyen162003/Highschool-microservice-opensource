using Domain.Enums;
using global::Domain.Common;

namespace Domain.Entities
{
    public class Document : BaseAuditableEntity
    {
        public Guid Id { get; set; }
        
        public string DocumentName { get; set; } = null!;

        public string? DocumentDescription { get; set; }        
        
        public int? View { get; set; }

        public string? Slug { get; set; }

        public int? Download { get; set; }

        public int? Like { get; set; }
        public Guid? CreatedBy { get; set; }   
        public Guid? UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }
        public Guid? SchoolId { get; set; }
        public Guid? SubjectCurriculumId { get; set; }
        public int? Semester { get; set; }
        public int? DocumentYear { get; set; }  
        public virtual ICollection<UserLike>? UserLikes { get; set; } = new List<UserLike>();
        public virtual School School { get; set; } = null!;
        public virtual SubjectCurriculum SubjectCurriculum { get; set; } = null!;
    }

}
