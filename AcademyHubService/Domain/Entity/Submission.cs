using Domain.CustomEntities;

namespace Domain.Entity;

public partial class Submission : BaseAuditableEntity
{
    public Guid Id { get; set; }

    public int? MemberId { get; set; }

    public Guid? AssignmentId { get; set; }

    public double? Score { get; set; }

    public virtual Assignment? Assignment { get; set; }

    public virtual ZoneMembership? Member { get; set; }
}
