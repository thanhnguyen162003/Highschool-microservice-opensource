namespace Domain.Entity;

public partial class Ket
{
    public Guid Id { get; set; }

    public Guid CreatedBy { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int? TotalQuestion { get; set; }

    public int? TotalPlay { get; set; }

    public string? Thumbnail { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? Author { get; set; }

    public virtual IEnumerable<KetContent> KetContents { get; set; } = new List<KetContent>();
}
