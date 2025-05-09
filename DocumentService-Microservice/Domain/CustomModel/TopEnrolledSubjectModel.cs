namespace Domain.CustomModel;

public class TopEnrolledSubjectModel
{
    public string Name { get; set; } = string.Empty;
    public int TotalEnrollmentCount { get; set; }
    public int Completion { get; set; }
}