using Domain.CustomEntities;

namespace Domain.Entity;

public partial class PendingZoneInvite : BaseAuditableEntity
{
    public int Id { get; set; }

    public Guid? ZoneId { get; set; }

    public string Email { get; set; } = null!;

    public Guid? InviteBy { get; set; }

    public string Type { get; set; } = null!;

    public DateTime? ExpiredAt { get; set; }

    public virtual Zone? Zone { get; set; }
}
