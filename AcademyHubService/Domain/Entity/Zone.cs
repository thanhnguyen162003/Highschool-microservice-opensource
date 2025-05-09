using Domain.CustomEntities;
using Domain.Enums;

namespace Domain.Entity;

public partial class Zone : BaseAuditableEntity
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? LogoUrl { get; set; }

    public string? BannerUrl { get; set; }
    public ZoneStatusEnum? Status { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public ICollection<Guid>? DocumentIds { get; set; } = new List<Guid>();

    public ICollection<Guid>? FlashcardIds { get; set; } = new List<Guid>();

    public ICollection<Guid>? FolderIds { get; set; } = new List<Guid>();

    public virtual ICollection<Assignment>? Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<PendingZoneInvite>? PendingZoneInvites { get; set; } = new List<PendingZoneInvite>();

    public virtual ICollection<ZoneBan>? ZoneBans { get; set; } = new List<ZoneBan>();

    public virtual ICollection<ZoneMembership>? ZoneMemberships { get; set; } = new List<ZoneMembership>();
}
