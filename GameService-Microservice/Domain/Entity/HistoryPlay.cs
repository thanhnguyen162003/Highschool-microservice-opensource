namespace Domain.Entity;

public partial class HistoryPlay
{
    public int Id { get; set; }

    public Guid? UserId { get; set; }

    public string? Avatar { get; set; }

    public string? DisplayName { get; set; }

    public int? Score { get; set; }

    public int? Rank { get; set; }

    public int? TimeAverage { get; set; }

    public DateTime? CreatedAt { get; set; }
}
