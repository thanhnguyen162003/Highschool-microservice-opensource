namespace SharedProject.ConsumeModel;

public class RecommendedDataModel
{
    public string? Id { get; set; }
    public List<Guid>? SubjectIds { get; set; }
    public Guid UserId { get; set; }
    public int Grade { get; set; }
    public string? TypeExam { get; set; }
}