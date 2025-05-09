namespace Application.Common.Models.DaprModel.Academic
{
    public class AcademicUserResponseDapr
    {
        public List<AcademicUserObjectDapr> Users { get; set; } = new List<AcademicUserObjectDapr>();
    }
    public class AcademicUserObjectDapr
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string Avatar { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
    }
}
