using Domain.CustomEntities;

namespace Domain.Entity;

public partial class ZoneMembership : BaseAuditableEntity
{
    public int Id { get; set; }

    public Guid? ZoneId { get; set; }

    public Guid? GroupId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? InviteBy { get; set; }

    public string Type { get; set; } = null!;

    public string? Email { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Group? Group { get; set; }

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();

    public virtual Zone? Zone { get; set; }
}
