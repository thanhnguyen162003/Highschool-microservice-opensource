using Domain.Entity;

namespace Application.Common.Models.AssignmentModel;

public partial class AssignmentResponseModel
{
    public Guid Id { get; set; }

    public Guid ZoneId { get; set; }

    public string? Type { get; set; }

    public string? Title { get; set; }

    public string? Noticed { get; set; }

    public int? TotalQuestion { get; set; }

    public int? TotalTime { get; set; }

    public DateTime? AvailableAt { get; set; }

    public DateTime? DueAt { get; set; }

    public DateTime? LockedAt { get; set; }

    public bool? Published { get; set; }

    public Guid? CreatedBy { get; set; }

    public int SubmissionsCount { get; set; }
    public bool? Submitted { get; set; }

}
