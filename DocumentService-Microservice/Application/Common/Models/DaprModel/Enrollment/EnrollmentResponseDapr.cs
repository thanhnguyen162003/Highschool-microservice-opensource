namespace Application.Common.Models.DaprModel.Enrollment
{
    public class EnrollmentResponseDapr
    {
        public List<EnrollmentObjectDapr> Enrollment { get; set; } = new List<EnrollmentObjectDapr>();
    }
    public class EnrollmentObjectDapr
    {
        public string UserId { get; set; }
        public List<string> LessonLearnDate { get; set; }
    }
}
