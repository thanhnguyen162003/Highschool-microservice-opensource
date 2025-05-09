namespace Domain.Entity;

public partial class ZoneBan
{
    public int Id { get; set; }

    public Guid? ZoneId { get; set; }

    public Guid? UserId { get; set; }

    public string Email { get; set; } = null!;

    public string? Reason { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Zone? Zone { get; set; }
}
