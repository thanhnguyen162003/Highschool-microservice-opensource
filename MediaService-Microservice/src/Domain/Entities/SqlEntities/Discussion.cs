
using Discussion_Microservice.Domain.Common;
using Domain.Entities.SqlEntites;
using Domain.Enums;

namespace Domain.Entities;
public class Discussion : BaseAuditableEntitySQL
{
    public Guid UserId { get; set; }
    public bool IsSolved { get; set; }
    public string Title { get; set; } = null!;
    public string? Content { get; set; } = null!;
    public string? ContentHtml { get; set; }
    public DiscussionStatus Status { get; set; }
    public string Slug { get; set; } = null!;
    public int Vote { get; set; }
    public Guid SubjectId { get; set; }
    public virtual ICollection<DiscussionTag> DiscussionTags { get; set; } = new List<DiscussionTag>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Upvote>? Upvotes { get; set; }

}
