namespace Domain.Entity;

public partial class Group
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public int? TotalPeople { get; set; }

    public Guid? Leader { get; set; }

    public virtual ICollection<ZoneMembership> ZoneMemberships { get; set; } = new List<ZoneMembership>();
}
