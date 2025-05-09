using Discussion_Microservice.Domain.Common;

namespace Domain.Entities.SqlEntites;
public class DiscussionTag : BaseAuditableEntitySQL
{
    public Guid DiscussionId { get; set; }
    public Guid TagId { get; set; }
    public virtual Discussion Discussion { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;

}
