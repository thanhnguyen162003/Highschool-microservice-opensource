namespace Domain.Entity;

public partial class KetContent
{
    public string? Answers { get; set; }

    public int? CorrectAnswer { get; set; }

    public Guid? KetId { get; set; }

    public string? Question { get; set; }

    public Guid? Id { get; set; }

    public int? Order { get; set; }

    public int? TimeAnswer { get; set; }

    public virtual Ket? Ket { get; set; }
}
