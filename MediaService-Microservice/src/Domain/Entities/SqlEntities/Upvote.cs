using Discussion_Microservice.Domain.Common;

namespace Domain.Entities.SqlEntites;
public class Upvote : BaseAuditableEntitySQL
{
    public Guid? DiscussionId { get; set; } = null!;
    public Guid? CommentId { get; set; } = null!;
    public Guid UserId { get; set; }
    public DateTime VoteTime { get; set; }
    public bool UpVoteComment { get; set; }
    public bool UpVoteDiscussion { get; set; }
    public virtual Discussion? Discussion { get; set; } = null!;
    public virtual Comment? Comment { get; set; } = null!;

}
