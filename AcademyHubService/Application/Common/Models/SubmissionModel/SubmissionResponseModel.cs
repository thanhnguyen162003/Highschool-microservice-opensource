using Application.Common.Models.ZoneModel;

namespace Application.Common.Models.SubmissionContent;

public class SubmissionResponseModel 
{
    public Guid Id { get; set; }

    public int? MemberId { get; set; }

    public Guid? AssignmentId { get; set; }

    public double? Score { get; set; }

    public Guid UserId { get; set; }

    public Author Learner { get; set; }
    public DateTime CreatedAt { get; set; }
}
