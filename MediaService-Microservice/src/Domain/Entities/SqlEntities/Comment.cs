using System.ComponentModel.DataAnnotations.Schema;
using Discussion_Microservice.Domain.Common;


namespace Domain.Entities.SqlEntites;
public class Comment : BaseAuditableEntitySQL
{
    public Guid UserId { get; set; }
    public Guid DiscussionId { get; set; }
    public string? Content { get; set; }
    public string? ContentHtml { get; set; }
    public string? AvatarImage { get; set; }
    public int? LikeNumber { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
    public Guid CreateBy { get; set; }
    public Guid UpdateBy { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsEdited { get; set; }
    [ForeignKey("DiscussionId")]
    public virtual Discussion Discussion { get; set; } = null!;
    public virtual ICollection<Upvote>? Upvotes { get; set; }
}
