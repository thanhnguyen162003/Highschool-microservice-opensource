using Discussion_Microservice.Domain.Common;

namespace Domain.Entities.SqlEntites;
public class Tag : BaseAuditableEntitySQL
{
    public string Name { get; set; } = null!;
    public virtual ICollection<DiscussionTag> DiscussionTags { get; set; } = null!;
}
