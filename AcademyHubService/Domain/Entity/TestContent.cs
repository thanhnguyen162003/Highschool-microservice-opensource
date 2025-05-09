namespace Domain.Entity;

public partial class TestContent
{
    public Guid Id { get; set; }

    public Guid? Assignmentid { get; set; }

    public string? Answers { get; set; }

    public int? CorrectAnswer { get; set; }

    public string? Question { get; set; }

    public int? Order { get; set; }

    public double? Score { get; set; }

    public virtual Assignment? Assignment { get; set; }
}
